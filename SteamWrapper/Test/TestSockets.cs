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
            
            //update loop
            while( true )
            {               
                m.Tick();               
                for(int i =0; i< 10;i++)
                {
                    var msg = String.Format( sendMsgFmt, index );
                    byte[] transportData = System.Text.Encoding.Default.GetBytes(msg);
                    var rsa = m.SendMessage( clientConnectId, transportData, (uint)transportData.Length,
                                             Constants.k_nSteamNetworkingSendFlags_Reliable );
                    if( rsa != EResult.k_EResultOK )
                    {
                        Console.WriteLine( "loop break, reason:{0}", rsa );
                        break;
                    }
                    index++;
                }

                var datas = m.ReceiveMessagesOnListenSocket( listenSocketId );
                Console.WriteLine("recieve data num:{0}", datas.Count);
                foreach(var data in datas)
                {
                    Console.WriteLine("recieve data:{0}", System.Text.Encoding.UTF8.GetString(data));    
                }  
                Thread.Sleep( 100 );
                
            }
            
            Console.ReadLine();
        }
    }
}