using System;
using System.Runtime.InteropServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SteamNetworkingSockets
{
    public static partial class Sockets
    { 
        public static EResult SendMessage(HSteamNetConnection hConn, byte[] pData, uint cbData, ESteamNetworkingSendType sendType)
        {
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(pData.Length);
            Marshal.Copy(pData, 0, unmanagedPointer, pData.Length);
            var rs = SendMessageToConnection(hConn, unmanagedPointer, cbData, sendType);
            Marshal.FreeHGlobal(unmanagedPointer);
            return rs;
        }
    }
}