using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace TCPServer
{
    class AsyncUserToken
    {
        public Socket m_kSocket { get; set; }
        public SocketAsyncEventArgs m_kSendEventArgs { get; set; }
        public SocketAsyncEventArgs m_kReceiveEventArgs { get; set; }
        private byte[] m_Databuff;
        private int m_nCurrentIndex;
        private int m_nCapacity;
        public bool m_bIsConnect { get; set; } 

        public AsyncUserToken()
        {

        }
        public void ReceiveData(SocketAsyncEventArgs e)
        {
            lock (m_Databuff)
            {
                if (m_nCapacity - m_nCurrentIndex < e.BytesTransferred)
                {
                    //容量不够了，严重错误
                    return;
                }

                Array.Copy(e.Buffer, e.Offset, m_Databuff, m_nCurrentIndex, e.BytesTransferred);
                m_nCurrentIndex += e.BytesTransferred;
            }
            
        }

        public void ProcessData()
        {
            lock (m_Databuff)
            {
                int i = 0;
                while (i < m_nCurrentIndex)
                {
                    int num = BitConverter.ToInt32(m_Databuff, i);
                    if (m_nCurrentIndex - i < num)
                    {
                        if (i != 0)
                        {
                            Array.Copy(m_Databuff, i, m_Databuff, 0, m_Databuff.Length - i);
                            m_nCurrentIndex -= i;
                        }
                        return;
                    }
                    short type = BitConverter.ToInt16(m_Databuff, i + 4);
                    //type = IPAddress.NetworkToHostOrder(type);
                    MemoryStream kStream = new MemoryStream(m_Databuff, i + 6, num - 6);
                    i += (int)num;
                    this.ProcessMsg(kStream, (int)type);
                }
                m_nCurrentIndex = 0;
            }
           
        }

        private void ProcessMsg(MemoryStream kStream, int eType)
        {
            MessageDispatcher.ProcessMsg(this, kStream, eType);
        }

        public void Send(string strMsg)
        {
            if (!m_bIsConnect)
            {
                return;
            }
            byte[] buffer = Encoding.UTF8.GetBytes(strMsg);
            m_kSendEventArgs.SetBuffer(buffer, 0, buffer.Length);
        }

        public void Init()
        {
            m_Databuff = new byte[1024];
            m_nCapacity = 1024;
            Reset();
        }

        public void Reset()
        {
            m_bIsConnect = true;
            m_nCurrentIndex = 0;
            m_kSocket = null;
        }
    }
}
