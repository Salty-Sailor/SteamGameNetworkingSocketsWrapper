using SteamNetworkingSockets;
using System;
using System.Threading;

namespace SteamWrapper.Test
{
    public static class TestSteam
    {
        static uint nConnectIP = 0x7f000001;//127.0.0.1
        static ushort nPort = 27200;

        public static void TestServer()
        {
            NetworkManager m = new NetworkManager(false, true);
            var listenSocketId = m.ListenSocket(-1, 0, nPort);
            var loop = true;
            while( loop )
            {
                m.Tick();
                m.ProcessPacket();
                Thread.Sleep( 100 );
            }
        }
               
        public static void TestClient()
        {
            NetworkManager m = new NetworkManager(true, false);
            var clientConn = m.Connect( nConnectIP, nPort );
            string sendMsgFmt = "test msg {0} send by client";
            var index = 0;
            var loop = true;
            //update loop
            while( loop )
            {
                m.Tick();               
                for(int i =0; i< 10;i++)
                {
                    var msg = String.Format( sendMsgFmt, index );
                    byte[] transportData = System.Text.Encoding.Default.GetBytes(msg);
                    Console.WriteLine( "Send:{0}", msg );
                    if( !clientConn.Send( transportData ) )
                    {
                        Console.WriteLine( "conn send fail" );
                        loop = false;
                    }
                    
                    index++;
                }
                
                Thread.Sleep( 100 );      
            }
        }
        
    }
}