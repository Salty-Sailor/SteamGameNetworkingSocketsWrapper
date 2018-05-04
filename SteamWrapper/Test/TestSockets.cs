using SteamNetworkingSockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SteamWrapper.Test
{
    public static class TestSockets
    {
        static uint nConnectIP = 0x7f000001;//127.0.0.1
        static ushort nPort = 27200;

        public static void ConnectEventsHandler(out List<uint> closeConn, out List<uint> acceptConn, out List<uint> connectedConn)
        {
            closeConn = new List<uint>();
            acceptConn = new List<uint>();
            connectedConn = new List<uint>();
            
            while(true)
            {                
                var close = Sockets.HandleConnectionClose();
                if( close == Sockets.k_HSteamNetConnection_Invalid )
                {
                    break;
                }
                Sockets.AcceptConnection( close );
                closeConn.Add( close );
            }
            
            while(true)
            {                
                var accept = Sockets.HandleConnectionAccept();
                if( accept == Sockets.k_HSteamNetConnection_Invalid )
                {
                    break;
                }
                Sockets.AcceptConnection( accept );
                acceptConn.Add( accept );
            }
                      
            while(true)
            {                
                var connected = Sockets.HandleConnectionConnected();
                if( connected == Sockets.k_HSteamNetConnection_Invalid )
                {
                    break;
                }
                Sockets.AcceptConnection( connected );
                connectedConn.Add( connected );
            }
        }
        
        public static void SimpleTest()
        {
            string reason = "";
            var succ = Sockets.GameNetworkingSockets_Init(ref reason);           
            Console.WriteLine("GameNetworkingSocketsInit:{0}", succ?"succ":"fail");
            var a = Sockets.NewSockets();
            var listenSocketId = Sockets.CreateListenSocket(a, -1, 0, nPort);
            var clientConnectId = Sockets.ConnectByIPv4Address(a, nConnectIP, nPort);

            var usecWhenStarted = Sockets.GetLocalTimestamp();
            var usecTestDuration = 120 * 1000000;
            var usecWorstCase = usecTestDuration + 30 * 1000000;
            long g_usecTestElapsed;

            string sendMsg = "test msg send by client";
            byte[] transportData = System.Text.Encoding.Default.GetBytes(sendMsg);

            byte[] recieveData = new byte[100];
            Task.Run(() =>
            {
                while (true)
                {
                    Sockets.TickCallBacks(a);
                    ConnectEventsHandler(out List<uint> closeConn, out List<uint> acceptConn, out List<uint> connectedConn);
                    
                    var now = Sockets.GetLocalTimestamp();
                    g_usecTestElapsed = now - usecWhenStarted;
                    var sendRs = Sockets.SendMessage(clientConnectId, transportData, (uint)transportData.Length, Sockets.ESteamNetworkingSendType.k_ESteamNetworkingSendType_Reliable);
                    IntPtr messageHandle = IntPtr.Zero;
                    var recieveRs = Sockets.ReceiveMessagesOnListenSocket(listenSocketId, ref messageHandle, 100);
                    Thread.Sleep(200);
                }
            });
            

            Console.ReadLine();
        }        
    }
}