using SteamNetworkingSockets;
using System;
using System.Threading;

namespace SteamWrapper.Test
{
    public static class TestSteam
    {
        static uint nConnectIP = 0x7f000001;//127.0.0.1
        static ushort nPort = 27200;

        public static void ManagerTest()
        {
            NetworkManager m = new NetworkManager();
            var listenSocketId = m.ListenSocket(-1, 0, nPort);
            var clientConnectId = m.Connect( nConnectIP, nPort );
            
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
                    var rsa = m.SendMessage( clientConnectId, transportData, (uint)transportData.Length,
                                             ESteamNetworkingSendType.k_ESteamNetworkingSendType_Reliable );
                    if( rsa != EResult.k_EResultOK )
                    {
                        Console.WriteLine( "loop break, reason:{0}", rsa );
                        loop = false;
                        break;
                    }
                    index++;
                }

                var datas = m.ReceiveMessagesOnListenSocket( listenSocketId );
                Console.WriteLine("recieve data num:{0}", datas.Count);
                for(int i=0;i<datas.Count;i++)
                {
                    Console.WriteLine("recieve data[{0}]:{1}", i, System.Text.Encoding.UTF8.GetString(datas[i]));    
                }  
                Thread.Sleep( 100 );             
            }
            m.Release();
            Console.ReadLine();
        }
    }
}