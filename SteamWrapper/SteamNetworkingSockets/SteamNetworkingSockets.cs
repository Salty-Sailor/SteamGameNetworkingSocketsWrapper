using System;
using System.Runtime.InteropServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;
namespace SteamNetworkingSockets
{
    public static class Steam
    {
        const string DllPath = @"C:\Users\xlt\WorkSpace\SteamGameNetworkingSocketsWrapper\SteamWrapper\GameNetworkingSockets.dll";

        [DllImport(DllPath, EntryPoint = "GameNetworkingSockets_Init", ThrowOnUnmappableChar = true)]
        public static extern bool GameNetworkingSockets_Init(IntPtr reason);

        [DllImport(DllPath, EntryPoint = "GameNetworkingSockets_Kill", ThrowOnUnmappableChar = true)]
        public static extern void GameNetworkingSockets_Kill();

        [DllImport(DllPath, EntryPoint = "SteamNetworkingSockets_GetLocalTimestamp", ThrowOnUnmappableChar = true)]
        public static extern SteamNetworkingMicroseconds GetLocalTimestamp();

        [DllImport(DllPath, EntryPoint = "SteamNetworkingSockets", ThrowOnUnmappableChar = true)]
        public static extern IntPtr NewSockets();

        [DllImport(DllPath, EntryPoint = "SteamNetworkingSocketsGameServer", ThrowOnUnmappableChar = true)]
        public static extern IntPtr NewSocketsGameServer();

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateListenSocket", ThrowOnUnmappableChar = true)]
        public static extern HSteamListenSocket CreateListenSocket(IntPtr ptr, int nSteamConnectVirtualPort, uint nIP, ushort nPort);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ConnectByIPv4Address", ThrowOnUnmappableChar = true)]
        public static extern HSteamNetConnection ConnectByIPv4Address(IntPtr ptr, uint nIP, ushort nPort);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_AcceptConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult AcceptConnection(IntPtr ptr, HSteamNetConnection hConn);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CloseConnection", ThrowOnUnmappableChar = true)]
        public static extern bool CloseConnection(IntPtr ptr, HSteamNetConnection hPeer, int nReason, string pszDebug, bool bEnableLinger);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CloseListenSocket", ThrowOnUnmappableChar = true)]
        public static extern bool CloseListenSocket(IntPtr ptr, HSteamListenSocket hSteamListenSocket, string pszNotifyRemoteReason);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetConnectionUserData", ThrowOnUnmappableChar = true)]
        public static extern bool SetConnectionUserData(IntPtr ptr, HSteamNetConnection hPeer, long nUserData);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionUserData", ThrowOnUnmappableChar = true)]
        public static extern long GetConnectionUserData(IntPtr ptr, HSteamNetConnection hPeer);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetConnectionName", ThrowOnUnmappableChar = true)]
        public static extern void SetConnectionName(IntPtr ptr, HSteamNetConnection hPeer, string pszName);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionName", ThrowOnUnmappableChar = true)]
        public static extern void GetConnectionName(IntPtr ptr, HSteamNetConnection hPeer, ref string pszName, int nMaxLen);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SendMessageToConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult SendMessageToConnection(IntPtr ptr, HSteamNetConnection hConn, IntPtr pData, uint cbData, ESteamNetworkingSendType sendType);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection", ThrowOnUnmappableChar = true)]
        public static extern EResult FlushMessagesOnConnection(IntPtr ptr, HSteamNetConnection hConn);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection", ThrowOnUnmappableChar = true)]
        public static extern int ReceiveMessagesOnConnection(IntPtr ptr, HSteamNetConnection hConn, IntPtr ppOutMessages, int nMaxMessages);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnListenSocket", ThrowOnUnmappableChar = true)]
        public static extern int ReceiveMessagesOnListenSocket(IntPtr ptr, HSteamListenSocket hConn, IntPtr ppOutMessages, int nMaxMessages);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionInfo", ThrowOnUnmappableChar = true)]
        public static extern bool GetConnectionInfo(IntPtr ptr, HSteamNetConnection hConn, IntPtr pInfo);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus", ThrowOnUnmappableChar = true)]
        public static extern bool GetQuickConnectionStatus(IntPtr ptr, HSteamNetConnection hConn, IntPtr pInfo);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus", ThrowOnUnmappableChar = true)]
        public static extern int GetDetailedConnectionStatus(IntPtr ptr, HSteamNetConnection hConn, ref string pInfo, int cbBuf);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetListenSocketInfo", ThrowOnUnmappableChar = true)]
        public static extern bool GetListenSocketInfo(IntPtr ptr,HSteamListenSocket hSocket, ref uint pnIP, ref ushort pnPort);

        [DllImport(DllPath, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateSocketPair", ThrowOnUnmappableChar = true)]
        public static extern bool CreateSocketPair(IntPtr ptr, ref IntPtr pInterface, ref HSteamNetConnection pOutConnection1, ref HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback);

#region callbacks
        [DllImport(DllPath, EntryPoint = "SteamNetworkingSockets_RunConnectionStatusChangedCallbacks", ThrowOnUnmappableChar = true)]
        public static extern bool RunConnectionStatusChangedCallbacks(IntPtr ptr, ref IntPtr pInterface, ref HSteamNetConnection pOutConnection1, ref HSteamNetConnection pOutConnection2, bool bUseNetworkLoopback);
#endregion

#region hooks

        [DllImport(DllPath, EntryPoint = "TickCallBacks", ThrowOnUnmappableChar = true)]
        public static extern void TickCallBacks(IntPtr pInterface);

        [DllImport(DllPath, EntryPoint = "ClearEventsQuene", ThrowOnUnmappableChar = true)]
        public static extern void ClearEventsQuene();

        [DllImport(DllPath, EntryPoint = "HandleConnectionAccept", ThrowOnUnmappableChar = true)]
        public static extern HSteamNetConnection HandleConnectionAccept();

        [DllImport(DllPath, EntryPoint = "HandleConnectionClose", ThrowOnUnmappableChar = true)]
        public static extern HSteamNetConnection HandleConnectionClose();

        [DllImport(DllPath, EntryPoint = "HandleConnectionConnected", ThrowOnUnmappableChar = true)]
        public static extern HSteamNetConnection HandleConnectionConnected();

#endregion

#region message handler

        [DllImport(DllPath, EntryPoint = "GetMessageSize", ThrowOnUnmappableChar = true)]
        public static extern uint GetMessageSize(IntPtr ptrTomessage);

        [DllImport(DllPath, EntryPoint = "GetMessagePayLoad", ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetMessagePayLoad(IntPtr ptrTomessage);

        [DllImport(DllPath, EntryPoint = "ReleaseMessage", ThrowOnUnmappableChar = true)]
        public static extern void ReleaseMessage(IntPtr ptrTomessage);

#endregion
    }
}
