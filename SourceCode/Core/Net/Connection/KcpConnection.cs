#if KCP
using System;
using System.Net.Sockets;

namespace Core.Net
{
    public class KcpConnection : AbstractKcpConnection
    {
        private Kcp kcp;

        public KcpConnection(IProtocolCoder protocolCoder, uint conv) : base(protocolCoder, conv)
        {
        }
        
        protected override void OnSend(byte[] buffs, int offset, int length)
        {
            if (null != socket)
            {
                kcp.Send(buffs, offset, length);
            }
        }

        protected override void CreateKcp()
        {
            kcp = new Kcp(conv, new IntPtr(conv));
            kcp.NoDelay(1, 10, 2, 1);
            kcp.WndSize(300, 300);
            kcp.SetMtu(MAX_BUFF_SIZE);
            kcp.SetOutput(OnKcpOutput);
        }

        private void OnKcpOutput(byte[] bytes, int size, object user)
        {
            if (null == socket || size == 0)
            {
                return;
            }
            try
            {
                socket.BeginSend(bytes, 0, size, SocketFlags.None, null, socket);
            }
            catch (Exception e)
            {
                ProcessException(e.ToString());
            }
        }

        protected override void ReleaseKcp()
        {
            kcp.Release();
            kcp = null;
        }

        protected override void KcpInput()
        {
            kcp.Input(recvBuffer.GetBuffer(), 0, recvBuffer.ReadableBytes());
        }

        protected override int KcpPeekSize()
        {
            return kcp.PeekSize();
        }

        protected override int KcpRecv(byte[] buffer, int offset, int length)
        {
            return kcp.Recv(buffer, 0, length);
        }

        protected override void UpdateKcp()
        {
            kcp.Update(elapseTime);
            kcp.Check(elapseTime);
        }

    }
}
#endif