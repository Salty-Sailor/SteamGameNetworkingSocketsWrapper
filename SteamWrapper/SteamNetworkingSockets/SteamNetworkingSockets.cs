using System;
using System.Runtime.InteropServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;
namespace SteamNetworkingSockets
{
    public static class Steam
    {
        [DllImport("libGameNetworkingSockets", EntryPoint = "GameNetworkingSockets_Init", ThrowOnUnmappableChar = true)]
        public static extern bool GameNetworkingSockets_Init(ref string reason);

        [DllImport("libGameNetworkingSockets", EntryPoint = "GameNetworkingSockets_Kill", ThrowOnUnmappableChar = true)]
        public static extern void GameNetworkingSockets_Kill();

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetLocalTimestamp", ThrowOnUnmappableChar = true)]
        public static extern SteamNetworkingMicroseconds GetLocalTimestamp();

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets", ThrowOnUnmappableChar = true)]
        public static extern IntPtr NewSockets();

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSocketsGameServer", ThrowOnUnmappableChar = true)]
        public static extern IntPtr NewSocketsGameServer();
        
        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_CreateListenSocket", ThrowOnUnmappableChar = true)]
        public static extern HSteamListenSocket CreateListenSocket(IntPtr ptr, int nSteamConnectVirtualPort, uint nIP, ushort nPort);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_ConnectByIPv4Address", ThrowOnUnmappableChar = true)]
        public static extern HSteamNetConnection ConnectByIPv4Address(IntPtr ptr, uint nIP, ushort nPort);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_AcceptConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult AcceptConnection(HSteamNetConnection hConn);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_CloseConnection", ThrowOnUnmappableChar = true)]
        public static extern bool CloseConnection(HSteamNetConnection hPeer, int nReason, string pszDebug, bool bEnableLinger);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_CloseListenSocket", ThrowOnUnmappableChar = true)]
        public static extern bool CloseListenSocket(HSteamListenSocket hSteamListenSocket, string pszNotifyRemoteReason);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_SetConnectionUserData", ThrowOnUnmappableChar = true)]
        public static extern bool SetConnectionUserData(HSteamNetConnection hPeer, long nUserData);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetConnectionUserData", ThrowOnUnmappableChar = true)]
        public static extern long GetConnectionUserData(HSteamNetConnection hPeer);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_SetConnectionName", ThrowOnUnmappableChar = true)]
        public static extern void SetConnectionName(HSteamNetConnection hPeer, string pszName);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetConnectionName", ThrowOnUnmappableChar = true)]
        public static extern void GetConnectionName(HSteamNetConnection hPeer, ref string pszName, int nMaxLen);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_SendMessageToConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult SendMessageToConnection(HSteamNetConnection hConn, IntPtr pData, uint cbData, ESteamNetworkingSendType sendType);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_FlushMessagesOnConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult FlushMessagesOnConnection(HSteamNetConnection hConn);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_ReceiveMessagesOnConnection", ThrowOnUnmappableChar = true)]
        public static extern int ReceiveMessagesOnConnection(HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_ReceiveMessagesOnListenSocket", ThrowOnUnmappableChar = true)]
        public static extern int ReceiveMessagesOnListenSocket(HSteamListenSocket hConn, IntPtr ppOutMessages, int nMaxMessages);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetConnectionInfo", ThrowOnUnmappableChar = true)]
        public static extern bool GetConnectionInfo(HSteamNetConnection hConn, IntPtr pInfo);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetQuickConnectionStatus", ThrowOnUnmappableChar = true)]
        public static extern bool GetQuickConnectionStatus(HSteamNetConnection hConn, IntPtr pInfo);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetDetailedConnectionStatus", ThrowOnUnmappableChar = true)]
        public static extern int GetDetailedConnectionStatus(HSteamNetConnection hConn, ref string pInfo, int cbBuf);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_GetListenSocketInfo", ThrowOnUnmappableChar = true)]
        public static extern bool GetListenSocketInfo(HSteamListenSocket hSocket, ref uint pnIP, ref ushort pnPort);

        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_CreateSocketPair", ThrowOnUnmappableChar = true)]
        public static extern bool CreateSocketPair(ref IntPtr pInterface, ref HSteamNetConnection pOutConnection1, ref HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback);

        #region callbacks
        [DllImport("libGameNetworkingSockets", EntryPoint = "SteamNetworkingSockets_RunConnectionStatusChangedCallbacks", ThrowOnUnmappableChar = true)]
        public static extern bool RunConnectionStatusChangedCallbacks(ref IntPtr pInterface, ref HSteamNetConnection pOutConnection1, ref HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback);
        #endregion

        #region hooks

        [DllImport( "libGameNetworkingSockets", EntryPoint = "TickCallBacks", ThrowOnUnmappableChar = true )]
        public static extern void TickCallBacks( IntPtr pInterface );

        [DllImport( "libGameNetworkingSockets", EntryPoint = "ClearEventsQuene", ThrowOnUnmappableChar = true )]
        public static extern void ClearEventsQuene();
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "HandleConnectionAccept", ThrowOnUnmappableChar = true )]
        public static extern HSteamNetConnection HandleConnectionAccept();
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "HandleConnectionClose", ThrowOnUnmappableChar = true )]
        public static extern HSteamNetConnection HandleConnectionClose();
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "HandleConnectionConnected", ThrowOnUnmappableChar = true )]
        public static extern HSteamNetConnection HandleConnectionConnected();
        
        #endregion
        
        #region message handler
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "GetMessageSize", ThrowOnUnmappableChar = true )]
        public static extern uint GetMessageSize(IntPtr ptrTomessage);
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "GetMessagePayLoad", ThrowOnUnmappableChar = true )]
        public static extern IntPtr GetMessagePayLoad(IntPtr ptrTomessage);
        
        [DllImport( "libGameNetworkingSockets", EntryPoint = "ReleaseMessage", ThrowOnUnmappableChar = true )]
        public static extern void ReleaseMessage(IntPtr ptrTomessage);
        
        #endregion
    }

   
}
