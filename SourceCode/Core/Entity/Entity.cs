using System;
using System.Collections.Generic;

namespace Core.Entity
{
    public class CreateArgs
    {
    }

    public abstract class Entity
    {
        private bool isStart;
        private bool isEnable;

        private List<EntityComponent> comps;
        private List<ITickable> ticks;
        private List<IUpdateable> updates;

        public IEntityManager Context { get; private set; }
        private float startElapseTime;
        private int startElapseFrame;

        public event Action<Entity> releaseEvent;

        public int InstId { get; private set; }

        public float ElapseTime
        {
            get { return Context.ElapseTime - startElapseTime; }
        }

        public int ElapseFrame
        {
            get { return Context.ElapseFrame - startElapseFrame; }
        }

        public Entity()
        {
            comps = new List<EntityComponent>();
            OnInitial();
        }

        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                if (isEnable != value)
                {
                    isEnable = value;
                    if (isEnable)
                    {
                        OnEnable();
                    }
                    else
                    {
                        OnDisable();
                    }

                    foreach (var item in comps)
                    {
                        item.OwnerChangeEnable();
                    }
                }
            }
        }

        internal void SetContext(IEntityManager context)
        {
            this.Context = context;
        }

        public void SetInstId(int inst)
        {
            InstId = inst;
        }

        internal void Start(CreateArgs args)
        {
            if (!isStart)
            {
                isStart = true;
                startElapseTime = Context.ElapseTime;
                startElapseFrame = Context.ElapseFrame;
                OnStart(args);
                foreach (var item in comps)
                {
                    item.Start();
                }

                IsEnable = true;
                OnAfterStart(args);
            }
        }

        /// <summary>
        /// 每一逻辑帧调用,外部不能调用该接口
        /// </summary>
        internal void Tick()
        {
            if (isEnable)
            {
                OnTick();
                if (null != ticks)
                {
                    foreach (var item in ticks)
                    {
                        if (((EntityComponent)item).IsEnable)
                        {
                            item.Tick();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 每一逻辑帧回滚调用,外部不能调用该接口
        /// </summary>
        internal void Rollback()
        {
            if (isEnable)
            {
                OnRollback();
                if (null != ticks)
                {
                    foreach (var item in ticks)
                    {
                        if (((EntityComponent)item).IsEnable)
                        {
                            item.Rollback();
                        }
                    }
                }
            }
        }

        internal void Update(float deltaTime)
        {
            if (isEnable)
            {
                OnUpdate(deltaTime);
                if (null != updates)
                {
                    foreach (var item in updates)
                    {
                        if (((EntityComponent)item).IsEnable)
                        {
                            item.Update(deltaTime);
                        }
                    }
                }
            }
        }

        internal void Release()
        {
            if (isStart)
            {
                if (null != releaseEvent)
                {
                    releaseEvent.Invoke(this);
                    releaseEvent = null;
                }

                IsEnable = false;
                isStart = false;
                foreach (var item in comps)
                {
                    item.Release();
                }

                OnRelease();
            }
        }

        internal void Dispose()
        {
            Release();
            foreach (var item in comps)
            {
                item.Dispose();
            }

            OnDispose();
            releaseEvent = null;
            comps = null;
            ticks = null;
            updates = null;
        }

        private int FindComponent<T>() where T : EntityComponent
        {
            Type type = typeof(T);
            for (int i = 0; i < comps.Count; i++)
            {
                if (comps[i].GetType() == type)
                {
                    return i;
                }
            }

            return -1;
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            int index = FindComponent<T>();
            if (index >= 0)
            {
                return (T)comps[index];
            }

            return null;
        }

        public T AddComponent<T>() where T : EntityComponent
        {
            T comp = GetComponent<T>();
            if (null != comp)
            {
                return comp;
            }

            comp = Activator.CreateInstance<T>();
            comps.Add(comp);
            if (comp is ITickable tick)
            {
                if (null == ticks)
                {
                    ticks = new List<ITickable>();
                }

                ticks.Add(tick);
            }

            if (comp is IUpdateable update)
            {
                if (null == updates)
                {
                    updates = new List<IUpdateable>();
                }

                updates.Add(update);
            }

            comp.Initial(this);
            if (isStart)
            {
                comp.Start();
            }

            return comp;
        }

        public void RemoveComponent<T>() where T : EntityComponent
        {
            int index = FindComponent<T>();
            if (index < 0)
            {
                return;
            }

            T comp = comps[index] as T;
            comps.RemoveAt(index);
            if (comp is ITickable tick)
            {
                ticks?.Remove(tick);
            }

            if (comp is IUpdateable update)
            {
                updates?.Remove(update);
            }

            if (isStart)
            {
                comp.Release();
            }

            comp.Dispose();
        }


        /// <summary>
        /// 在构造初始化调用
        /// </summary>
        protected virtual void OnInitial()
        {
        }

        /// <summary>
        /// 开始时调用一次
        /// </summary>
        protected virtual void OnStart(CreateArgs args)
        {
        }

        /// <summary>
        /// 开始时调用一次
        /// </summary>
        protected virtual void OnAfterStart(CreateArgs args)
        {
        }

        /// <summary>
        /// 激活时调用一次
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// 每逻辑帧调用一次
        /// </summary>
        protected virtual void OnTick()
        {
        }

        /// <summary>
        /// 每帧回滚调用一次
        /// </summary>
        protected virtual void OnRollback()
        {
        }

        /// <summary>
        /// 每渲染帧调用一次
        /// </summary>
        /// <param name="deltaTime"></param>
        protected virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 关闭时调用一次
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// 释放时调用一次
        /// </summary>
        protected virtual void OnRelease()
        {
        }

        /// <summary>
        /// 删除时调用一次
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}