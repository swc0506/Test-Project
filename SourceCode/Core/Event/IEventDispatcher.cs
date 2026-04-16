using System;

namespace Core.Event
{
    internal interface IEventDispatcher<K>
    {
        /// <summary>
        /// 加入监听事件不带参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        void AddListener(K id, Action callback);

        /// <summary>
        /// 加入监听事件带1个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型</typeparam>
        void AddListener<T1>(K id, Action<T1> callback);

        /// <summary>
        /// 加入监听事件带2个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        void AddListener<T1, T2>(K id, Action<T1, T2> callback);

        /// <summary>
        /// 加入监听事件带3个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        /// <typeparam name="T3">方法参数类型3</typeparam>
        void AddListener<T1, T2, T3>(K id, Action<T1, T2, T3> callback);

        /// <summary>
        /// 加入监听事件带4个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        /// <typeparam name="T3">方法参数类型3</typeparam>
        /// <typeparam name="T4">方法参数类型4</typeparam>
        void AddListener<T1, T2, T3, T4>(K id, Action<T1, T2, T3, T4> callback);


        /// <summary>
        /// 移除监听事件不带参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        void RemoveListener(K id, Action callback);

        /// <summary>
        /// 移除监听事件带1参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        void RemoveListener<T1>(K id, Action<T1> callback);

        /// <summary>
        /// 移除监听事件带2参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        void RemoveListener<T1, T2>(K id, Action<T1, T2> callback);

        /// <summary>
        /// 移除监听事件带3参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        /// <typeparam name="T3">方法参数类型3</typeparam>
        void RemoveListener<T1, T2, T3>(K id, Action<T1, T2, T3> callback);

        /// <summary>
        /// 移除监听事件带4参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="callback">方法</param>
        /// <typeparam name="T1">方法参数类型1</typeparam>
        /// <typeparam name="T2">方法参数类型2</typeparam>
        /// <typeparam name="T3">方法参数类型3</typeparam>
        /// <typeparam name="T4">方法参数类型4</typeparam>
        void RemoveListener<T1, T2, T3, T4>(K id, Action<T1, T2, T3, T4> callback);


        /// <summary>
        /// 派发监听事件
        /// </summary>
        /// <param name="id"></param>
        void DispatchListener(K id);

        /// <summary>
        /// 派发监听事件带1个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="args1">参数1</param>
        /// <typeparam name="T1">参数1类型</typeparam>
        void DispatchListener<T1>(K id, T1 args1);

        /// <summary>
        /// 派发监听事件带2个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="args1">参数1</param>
        /// <param name="args2">参数2</param>
        /// <typeparam name="T1">参数1类型</typeparam>
        /// <typeparam name="T2">参数2类型</typeparam>
        void DispatchListener<T1, T2>(K id, T1 args1, T2 args2);

        /// <summary>
        /// 派发监听事件带3个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="args1">参数1</param>
        /// <param name="args2">参数2</param>
        /// <param name="args3">参数3</param>
        /// <typeparam name="T1">参数1类型</typeparam>
        /// <typeparam name="T2">参数2类型</typeparam>
        /// <typeparam name="T3">参数3类型</typeparam>
        void DispatchListener<T1, T2, T3>(K id, T1 args1, T2 args2, T3 args3);

        /// <summary>
        /// 派发监听事件带4个参数
        /// </summary>
        /// <param name="id">事件id</param>
        /// <param name="args1">参数1</param>
        /// <param name="args2">参数2</param>
        /// <param name="args3">参数3</param>
        /// <param name="args4">参数4</param>
        /// <typeparam name="T1">参数1类型</typeparam>
        /// <typeparam name="T2">参数2类型</typeparam>
        /// <typeparam name="T3">参数3类型</typeparam>
        /// <typeparam name="T4">参数4类型</typeparam>
        void DispatchListener<T1, T2, T3, T4>(K id, T1 args1, T2 args2, T3 args3, T4 args4);

        /// <summary>
        /// 清除所有监听事件,注意:不支持匿名函数类型
        /// </summary>
        /// <param name="caller">执行对象</param>
        void ClearListenerByCaller(object caller);
        
        /// <summary>
        /// 清除所有监听事件
        /// </summary>
        /// <param name="id"></param>
        void ClearListener(K id);

        /// <summary>
        /// 清除所有事件
        /// </summary>
        void ClearAll();
    }
}