﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SteamNetworkingSockets
{
    public class Connection
    {
        private HSteamNetConnection _Id;

        public HSteamNetConnection Id
        {
            get { return _Id; }
        }

        private bool _IsConnected;

        public bool IsConnected
        {
            set { _IsConnected = value; }

            get { return _IsConnected; }
        }

        public Connection( HSteamNetConnection id )
        {
            _Id = id;
            _IsConnected = false;
        }
    }

    public class NetworkManager
    {
        private Dictionary<HSteamNetConnection, Connection> Conns;
        //Just keep same with the SteamNetworkingSocketslib, still thinking about how to use them
        public IntPtr UserSocket;
        public IntPtr GameServerSocket;

        private string initRs = "";
        private IntPtr messageBuffer;

        //temporary set this max number
        private static int MaxMessages = 1000 * 1000;
        
        public NetworkManager()
        {
            Steam.GameNetworkingSockets_Init( ref initRs );
            Console.WriteLine( "Init Steam GameNetworkingSockets:{0}", initRs );
            Conns = new Dictionary<HSteamNetConnection, Connection>();
            UserSocket = Steam.NewSockets();
            GameServerSocket = Steam.NewSocketsGameServer();
            messageBuffer = Marshal.AllocHGlobal( MaxMessages * IntPtr.Size );
        }

        public void Release()
        {
            Steam.GameNetworkingSockets_Kill();
            Marshal.AllocHGlobal(messageBuffer);
        }
        
        #region Connections CRUD

        public bool RemoveConnection( HSteamNetConnection id )
        {
            return Conns.Remove( id );
        }

        public bool AddConn( HSteamNetConnection id )
        {
            if( id == Constants.k_HSteamNetConnection_Invalid )
            {
                return false;
            }

            Conns.Add( id, new Connection( id ) );
            return true;
        }

        public Connection TryGet( HSteamNetConnection id )
        {
            if( Conns.ContainsKey( id ) )
            {
                return Conns[id];
            }

            return null;
        }

        public bool TrySet( HSteamNetConnection id, bool isConnected )
        {
            if( Conns.ContainsKey( id ) )
            {
                Conns[id].IsConnected = isConnected;
                return true;
            }

            return false;
        }

        #endregion

        public EResult SendMessage( HSteamNetConnection hConn, byte[] pData, uint cbData, int sendType )
        {
            IntPtr unmanagedPointer = Marshal.AllocHGlobal( pData.Length );
            Marshal.Copy( pData, 0, unmanagedPointer, pData.Length );
            var rs = Steam.SendMessageToConnection( hConn, unmanagedPointer, cbData, sendType );
            Marshal.FreeHGlobal( unmanagedPointer );
            return rs;
        }

        private const int MessagesLength = 56; //Bytes

        public List<byte[]> ReceiveMessagesOnListenSocket( HSteamListenSocket hConn, int nMaxMessages = 100 )
        {
            var rs = new List<byte[]>();
            int num = Steam.ReceiveMessagesOnListenSocket( hConn, messageBuffer, nMaxMessages );            
            for( int i = 0; i < num; i++ )
            {
                IntPtr messagePtr = IntPtr.Add( messageBuffer, IntPtr.Size * i );
                int size = (int)Steam.GetMessageSize( messagePtr );
                if( size == 0 )
                {
                    Steam.ReleaseMessage( messagePtr );
                    continue;
                }

                IntPtr unManagedData = Steam.GetMessagePayLoad( messagePtr );
                if(unManagedData == IntPtr.Zero)
                {
                    Steam.ReleaseMessage( messagePtr );
                    continue;
                }
                byte[] data = new byte[size];
                Marshal.Copy( unManagedData, data, 0, size );
                Steam.ReleaseMessage( messagePtr );
                rs.Add( data );              
            }
            return rs;
        }

        public HSteamListenSocket ListenSocket( int nSteamConnectVirtualPort, uint nIP, ushort nPort )
        {
            return Steam.CreateListenSocket( UserSocket, nSteamConnectVirtualPort, nIP, nPort );
        }

        public HSteamNetConnection Connect( uint nIP, ushort nPort )
        {
            HSteamNetConnection conId = Steam.ConnectByIPv4Address( UserSocket, nIP, nPort );
            return conId;
        }

        public void Tick()
        {
            Steam.TickCallBacks( UserSocket );
            while( true )
            {
                var closeId = Steam.HandleConnectionClose();
                if( closeId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                Console.WriteLine( "close:{0}", closeId );
                RemoveConnection( closeId );
            }

            while( true )
            {
                var knockId = Steam.HandleConnectionAccept();
                if( knockId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                Console.WriteLine( "knock:{0}", knockId );
                Steam.AcceptConnection( knockId );
                AddConn( knockId );
            }

            while( true )
            {
                var connectId = Steam.HandleConnectionConnected();
                if( connectId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                Console.WriteLine( "connected:{0}", connectId );
                TrySet( connectId, true );
            }
        }
    }
}