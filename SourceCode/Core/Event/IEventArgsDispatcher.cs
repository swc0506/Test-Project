using System;

namespace Core.Event
{
    internal interface IEventArgsDispatcher<K>
    {
        /// <summary>
        /// 加入监听事件
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        void AddListener(K id, EventHandler<EventArgs> callback);

        /// <summary>
        /// 移除监听事件
        /// </summary>
        /// <param name="eventId">事件id</param>
        /// <param name="callback">方法</param>
        void RemoveListener(K id, EventHandler<EventArgs> callback);

        /// <summary>
        /// 触发监听事件
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="sender">发送者</param>
        /// <param name="args">参数</param>
        void DispatchListener(K id, object sender, EventArgs args);

        /// <summary>
        /// 清除所有监听事件,注意:不支持匿名函数类型
        /// </summary>
        /// <param name="caller">执行域</param>
        void ClearListenerByCaller(object caller);
        
        /// <summary>
        /// 清除所有监听事件
        /// </summary>
        /// <param name="id">事件id</param>
        void ClearListener(K id);
        
        /// <summary>
        /// 清楚所有事件
        /// </summary>
        void ClearAll();
    }
}