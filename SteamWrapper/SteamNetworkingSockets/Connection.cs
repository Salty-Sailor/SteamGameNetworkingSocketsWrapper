using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSteamListenSocket = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SteamNetworkingSockets
{
    public class LocalConnection:Connection
    {
        public LocalConnection( HSteamNetConnection mId, NetworkManager manager ):base( mId, manager )
        { }

        public override bool Send( byte[] packet )
        {
            if( m_IsClosed )
            {
                return false;
            }

            m_NetworkData.Enqueue( packet );
            return true;
        }

        public override byte[] Receive()
        {
            if( m_NetworkData.Count != 0 )
            {
                return m_NetworkData.Dequeue();
            }

            if( m_IsClosed )
            {
                return null;
            }

            return NoMessage;
        }

        public override void Close()
        {
            m_IsClosed = true;
        }

        public override bool IsClosed
        {
            get { return m_IsClosed; }
        }
    }

    public class Connection
    {
        private NetworkManager m_NetworkManager;

        public static byte[] NoMessage = new byte[1] { 0 };

        public ESteamNetworkingSendType SendType = ESteamNetworkingSendType.k_ESteamNetworkingSendType_Reliable;

        private HSteamNetConnection m_ID;

        public HSteamNetConnection ID
        {
            get { return m_ID; }
        }

        protected Queue<byte[]> m_NetworkData;

        protected bool m_IsConnected;

        public bool IsConnected
        {
            set { m_IsConnected = value; }

            get { return m_IsConnected; }
        }

        protected object m_Lock = new object();

        protected bool m_IsClosed;

        public virtual bool IsClosed
        {
            get
            {
                lock( m_Lock )
                {
                    return m_IsClosed;
                }
            }
        }

        public object UserData { get; set; }

        public bool IsEmptyPacket( byte[] packet )
        {
            return ReferenceEquals( packet, NoMessage );
        }

        protected internal void forward( byte[] packet )
        {
            lock( m_Lock )
            {
                m_NetworkData.Enqueue( packet );
            }
        }

        public virtual bool Send( byte[] packet )
        {
            if( packet == null || packet.Length == 0 )
            {
                throw new ArgumentException( "Can not send null packet or zero size packet" );
            }

            var eResult = m_NetworkManager.SendMessage( m_ID, packet, (uint)packet.Length, SendType );
            if( eResult != EResult.k_EResultOK )
            {
                return false;
            }

            return true;
        }

        public virtual byte[] Receive()
        {
            lock( m_Lock )
            {
                if( m_NetworkData.Count != 0 )
                {
                    return m_NetworkData.Dequeue();
                }

                if( m_IsClosed )
                {
                    return null;
                }

                return NoMessage;
            }
        }

        public virtual void Close()
        {
            lock( m_Lock )
            {
                if( m_IsClosed )
                {
                    return;
                }

                m_IsClosed = true;
                //TODO：close the connection in C++ side
            }
        }

        public virtual void Flush()
        {
            lock( m_Lock )
            {
                //TODO: set a pending buffer to store some send packet
            }
        }

        public Connection( HSteamNetConnection mId, NetworkManager manager )
        {
            m_ID = mId;
            m_NetworkManager = manager;
            m_IsConnected = false;
            m_NetworkData = new Queue<byte[]>();
        }

        protected Connection()
        { }
    }
}