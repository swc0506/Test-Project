#if FANTASY_NET
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KCP;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Fantasy;

/// <summary>
/// KCP 服务器网络通道，用于处理服务器与客户端之间的数据通信。
/// </summary>
public class KCPServerNetworkChannel : ANetworkServerChannel
{
    private uint _channelId;
    private int _maxSndWnd;
    private readonly Socket _socket;
    public readonly uint CreateTime;
    private MemoryStream _receiveMemoryStream;
    private KCPServerNetwork _kcpServerNetwork;
    private readonly byte[] _channelIdBytes = new byte[sizeof(uint)];
    public Kcp Kcp { get; private set; }
    public uint ChannelId 
    {
        get => _channelId;
        private set
        {
            _channelId = value;
            _channelId.GetBytes(_channelIdBytes);
        }
    }

    /// <summary>
    /// 构造函数，创建 KCP 服务器网络通道实例。
    /// </summary>
    /// <param name="network"></param>
    /// <param name="id"></param>
    /// <param name="remoteEndPoint"></param>
    /// <param name="socket"></param>
    /// <param name="createTime"></param>
    public KCPServerNetworkChannel(KCPServerNetwork network, uint id, EndPoint remoteEndPoint, Socket socket, uint createTime) : base(network, id, remoteEndPoint)
    {
        ChannelId = id;
        _socket = socket;
        CreateTime = createTime;
        _kcpServerNetwork = network;
        Kcp = KCPFactory.Create(network.KcpSettings, id, KcpSpanCallback);
        _maxSndWnd = network.KcpSettings.MaxSendWindowSize;
    }

    /// <summary>
    /// 释放网络通道。
    /// </summary>
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        
        IsDisposed = true;
        InnerDispose();
    }

    private void InnerDispose()
    {
        if (_kcpServerNetwork.NetworkThread != Thread.CurrentThread)
        {
            _kcpServerNetwork.NetworkThreadSynchronizationContext.Post(Dispose);
            return;
        }
        
        Kcp = null;

        if (_socket.Connected)
        {
            var buff = new byte[5];
            buff.WriteTo(0, (byte)KcpHeader.Disconnect);
            buff.WriteTo(1, Id);
            _socket.SendTo(buff, 5, SocketFlags.None, RemoteEndPoint);
        }

        _maxSndWnd = 0;
        _receiveMemoryStream.Dispose();
        _receiveMemoryStream = null;
        // 移除网络通道
        _kcpServerNetwork.RemoveChannel(Id);
        // 因为当前是在网络线程中执行的，所以需要投递到Scene线程执行
        ChannelSynchronizationContext.Post(() =>
        {
            base.Dispose();
        });
    }
    
    /// <summary>
    /// 发送数据到客户端。
    /// </summary>
    /// <param name="memoryStream">内存流</param>
    public override void Send(MemoryStream memoryStream)
    {
        if (IsDisposed)
        {
            return;
        }
        
        // 投递到网络线程执行
        _kcpServerNetwork.NetworkThreadSynchronizationContext.Post(() =>
        {
            // 检查等待发送的消息，如果超出两倍窗口大小，KCP作者给的建议是要断开连接
            
            var waitSendSize = Kcp.WaitSendCount;

            if (waitSendSize > _maxSndWnd)
            {
                Log.Warning($"ERR_KcpWaitSendSizeTooLarge {waitSendSize} > {_maxSndWnd}");
                Dispose();
                return;
            }
        
            // 发送消息
            Kcp.Send(memoryStream.GetBuffer(),0, (int)memoryStream.Length);
            // 因为memoryStream对象池出来的、所以需要手动回收下
            memoryStream.Dispose();
            _kcpServerNetwork.AddToUpdate(0, Id);
        });
    }

    /// <summary>
    /// 发送数据到客户端。
    /// </summary>
    /// <param name="rpcId"></param>
    /// <param name="routeTypeOpCode"></param>
    /// <param name="routeId"></param>
    /// <param name="memoryStream"></param>
    /// <param name="message"></param>
    public override void Send(uint rpcId, long routeTypeOpCode, long routeId, MemoryStream memoryStream, object message)
    {
        Send(PacketParser.Pack(rpcId, routeTypeOpCode, routeId, memoryStream, message));
    }

    /// <summary>
    /// 接收数据并进行处理。
    /// </summary>
    public void Receive()
    {
        if (IsDisposed)
        {
            return;
        }

        while (true)
        {
            try
            {
                // 获得一个完整消息的长度

                var peekSize = Kcp.PeekSize();
                
                // 如果没有接收的消息那就跳出当前循环。

                if (peekSize < 0)
                {
                    return;
                }

                // 如果为0，表示当前消息发生错误。提示下、一般情况不会发生的

                if (peekSize == 0)
                {
                    throw new Exception("SocketError.NetworkReset");
                }

                _receiveMemoryStream = MemoryStreamHelper.GetRecyclableMemoryStream();
                _receiveMemoryStream.SetLength(peekSize);
                _receiveMemoryStream.Seek(0, SeekOrigin.Begin);

                var receiveCount = Kcp.Receive(_receiveMemoryStream.GetBuffer(), peekSize);

                // 如果接收的长度跟peekSize不一样，不需要处理，因为消息肯定有问题的(虽然不可能出现)。

                if (receiveCount != peekSize)
                {
                    Log.Error($"receiveCount != peekSize receiveCount:{receiveCount} peekSize:{peekSize}");
                    break;
                }

                if (!PacketParser.UnPack(_receiveMemoryStream, out var packInfo))
                {
                    break;
                }
                
                // 投递到Scene所在的线程执行
                
                ChannelSynchronizationContext.Post(() =>
                {
                    Session.Receive(packInfo);
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
    
    private const byte KcpHeaderReceiveData = (byte)KcpHeader.ReceiveData;
    
    private unsafe void KcpSpanCallback(byte[] buffer, ref int count)
    {
        if (IsDisposed)
        {
            return;
        }

        try
        {
            if (count == 0)
            {
                throw new Exception("KcpOutput count 0");
            }

            fixed (byte* p = buffer)
            {
                p[0] = KcpHeaderReceiveData;
                p[1] = _channelIdBytes[0];
                p[2] = _channelIdBytes[1];
                p[3] = _channelIdBytes[2];
                p[4] = _channelIdBytes[3];
            }
            
            _socket.SendTo(buffer, 0, count + 5, SocketFlags.None, RemoteEndPoint);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}
#endif