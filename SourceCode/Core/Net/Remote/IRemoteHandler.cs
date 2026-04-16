using System;

namespace Core.Net
{
    public interface IRemoteHandler : IMessageHandler, IDisposable
    {
        void OnConnect(int error);

        void OnDisconnect();

        void OnError(string errorMsg);

        void OnReconnect(int cur, int totalCount);
    }
}