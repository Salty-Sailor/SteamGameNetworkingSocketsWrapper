using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SteamNetworkingSockets
{
    public class NetworkManager
    {
        private Dictionary<HSteamNetConnection, Connection> Conns;
        private LocalConnection localConn;

        //Just keep same with the SteamNetworkingSocketslib, still thinking about how to use them
        private IntPtr UserSocket;

        //public IntPtr GameServerSocket;
        private bool isClient;
        private bool isServer;

        private IntPtr initRs = Marshal.AllocHGlobal(1024);
        private IntPtr messageBuffer;
        private IntPtr oneMessageBuffer;

        //temporary set this max number
        private static int MaxMessages = 1000 * 1000;

        public NetworkManager( bool isClient, bool isServer )
        {
            var r = Steam.GameNetworkingSockets_Init(initRs);
            if (!r)
            {
                Console.WriteLine("Init Steam GameNetworkingSockets Err:{0}", initRs);
                return;
            }

            this.isClient = isClient;
            this.isServer = isServer;
            Conns = new Dictionary<HSteamNetConnection, Connection>();
            UserSocket = Steam.NewSockets();

            messageBuffer = Marshal.AllocHGlobal( MaxMessages * IntPtr.Size );
            oneMessageBuffer = Marshal.AllocHGlobal( IntPtr.Size );
        }

        public void Release()
        {
            Steam.GameNetworkingSockets_Kill();
            Marshal.AllocHGlobal( messageBuffer );
            Marshal.AllocHGlobal( oneMessageBuffer );
        }

        #region Connections CRUD

        public bool CloseConn( HSteamNetConnection id )
        {
            if( Conns.ContainsKey( id ) )
            {
                Conns[id].Close();
            }

            return Conns.Remove( id );
        }

        public Connection AddConn( HSteamNetConnection id )
        {
            if( id == Constants.k_HSteamNetConnection_Invalid )
            {
                return null;
            }

            if( Conns.ContainsKey( id ) )
            {
                return Conns[id];
            }
            var newConn = new Connection( id, this );
            Conns.Add( id, newConn );
            return newConn;
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

        public Connection GetOrCreateLocalConn()
        {
            if( localConn == null )
            {
                localConn = new LocalConnection( 0, this );
            }

            return localConn;
        }

        #endregion

        #region MessageHandle

        public EResult SendMessage( HSteamNetConnection hConn, byte[] pData, uint cbData, ESteamNetworkingSendType sendType )
        {
            IntPtr unmanagedPointer = Marshal.AllocHGlobal( pData.Length );
            Marshal.Copy( pData, 0, unmanagedPointer, pData.Length );
            var rs = Steam.SendMessageToConnection(UserSocket, hConn, unmanagedPointer, cbData, sendType );
            Marshal.FreeHGlobal( unmanagedPointer );
            return rs;
        }

        public List<byte[]> ReceiveMessagesOnConnection( HSteamNetConnection hConn, int nMaxMessages = 100 )
        {
            nMaxMessages = nMaxMessages > MaxMessages ? MaxMessages : nMaxMessages;
            var rs = new List<byte[]>();
            int num = Steam.ReceiveMessagesOnConnection(UserSocket, hConn, messageBuffer, nMaxMessages );
            var ptr = messageBuffer.ToInt64();
            for( int i = 0; i < num; i++ )
            {
                IntPtr messagePtr = (IntPtr)(ptr + IntPtr.Size * i);
                //IntPtr messagePtr = IntPtr.Add( messageBuffer, IntPtr.Size * i );
                int size = (int)Steam.GetMessageSize( messagePtr );
                if( size == 0 )
                {
                    Steam.ReleaseMessage( messagePtr );
                    continue;
                }

                IntPtr unManagedData = Steam.GetMessagePayLoad( messagePtr );
                if( unManagedData == IntPtr.Zero )
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

        public List<byte[]> ReceiveMessagesOnListenSocket( HSteamListenSocket hSocket, int nMaxMessages = 100 )
        {
            nMaxMessages = nMaxMessages > MaxMessages ? MaxMessages : nMaxMessages;
            var rs = new List<byte[]>();
            int num = Steam.ReceiveMessagesOnListenSocket(UserSocket, hSocket, messageBuffer, nMaxMessages );
            var ptr = messageBuffer.ToInt64();
            for( int i = 0; i < num; i++ )
            {
                IntPtr messagePtr = (IntPtr)(ptr + IntPtr.Size * i);
                int size = (int)Steam.GetMessageSize( messagePtr );
                if( size == 0 )
                {
                    Steam.ReleaseMessage( messagePtr );
                    continue;
                }

                IntPtr unManagedData = Steam.GetMessagePayLoad( messagePtr );
                if( unManagedData == IntPtr.Zero )
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

        #endregion

        public HSteamListenSocket ListenSocket( int nSteamConnectVirtualPort, uint nIP, ushort nPort )
        {
            return Steam.CreateListenSocket( UserSocket, nSteamConnectVirtualPort, nIP, nPort );
        }

        public Connection Connect( uint nIP, ushort nPort )
        {
            HSteamNetConnection conId = Steam.ConnectByIPv4Address( UserSocket, nIP, nPort );
            return AddConn( conId );
        }

        public void Tick()
        {
            Steam.TickCallBacks( UserSocket );
            HandleConnectEvents();
            ForwardMessage();
        }

        void HandleConnectEvents()
        {
            while( true )
            {
                var closeId = Steam.HandleConnectionClose();
                if( closeId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                CloseConn( closeId );
            }

            while( true )
            {
                var knockId = Steam.HandleConnectionAccept();
                if( knockId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                Steam.AcceptConnection(UserSocket, knockId );
                AddConn( knockId );
            }

            while( true )
            {
                var connectId = Steam.HandleConnectionConnected();
                if( connectId == Constants.k_HSteamNetConnection_Invalid )
                {
                    break;
                }

                TrySet( connectId, true );
            }
        }

        void ForwardMessage()
        {
            foreach( var conn in Conns.Values )
            {
                var messages = ReceiveMessagesOnConnection( conn.ID );
                foreach( var msg in messages )
                {
                    conn.forward( msg );
                }
            }
        }

        void serverCallBack( Connection conn )
        {
            byte[] rsp = System.Text.Encoding.Default.GetBytes( "OK" );
            conn.Send( rsp );
        }

        //
        public void ProcessPacket()
        {
            foreach( var conn in Conns.Values )
            {
                while( true )
                {
                    var packet = conn.Receive();
                    if( conn.IsEmptyPacket( packet ) )
                    {
                        break;
                    }

                    serverCallBack( conn );
                    Console.WriteLine( "recv:{0}", System.Text.Encoding.Default.GetString( packet ) );
                }
            }
        }
    }
}