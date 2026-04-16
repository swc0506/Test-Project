using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Entity
{
    public abstract class EntityManager<T> : IEntityManager, IEnumerable<T> where T : Entity
    {
        private Queue<T> pool;

        private int createId;
        protected List<T> entities = new List<T>();

        private List<CreateArgs> creates = new List<CreateArgs>();
        private List<int> releases = new List<int>();

        public int ElapseFrame { get; private set; }
        public float ElapseTime { get; private set; }


        public event Action<T> createEvent;
        public event Action<T> releaseEvent;

        public int Count
        {
            get { return entities.Count; }
        }

        public EntityManager()
        {
            pool = new Queue<T>();
            ElapseFrame = 0;
            ElapseTime = 0;
            OnInitial();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 创建对象 注:不能在Entity内调用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Entity Create(CreateArgs args)
        {
            T entity = null;
            if (pool.Count > 0)
            {
                entity = pool.Dequeue();
            }
            else
            {
                entity = Activator.CreateInstance<T>();
                entity.SetContext(this);
                OnNewEntity(entity);
            }

            entities.Add(entity);
            entity.SetInstId(++createId);
            entity.Start(args);
            createEvent?.Invoke(entity);
            return entity;
        }

        protected abstract void OnNewEntity(T entity);

        /// <summary>
        /// 释放实例对象 注:不能在Entity的Tick方法内调用
        /// </summary>
        /// <param name="instId"></param>
        public void Release(int instId)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var item = entities[i];
                if (item.InstId == instId)
                {
                    entities.RemoveAt(i);
                    ReleaseEntity(item);
                    break;
                }
            }
        }

        private void ReleaseEntity(T entity)
        {
            pool.Enqueue(entity);
            releaseEvent?.Invoke(entity);
            entity.Release();
        }

        public T Get(int instId)
        {
            foreach (var item in entities)
            {
                if (item.InstId == instId)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建实力对象 可在Entity的Tick方法内调用
        /// </summary>
        /// <param name="args"></param>
        public void PutCreate(CreateArgs args)
        {
            creates.Add(args);
        }

        /// <summary>
        /// 释放实例对象 可在Entity的Tick方法内调用
        /// </summary>
        /// <param name="instId"></param>
        public void PutRelease(int instId)
        {
            releases.Add(instId);
        }

        /// <summary>
        /// 释放所有实例对象
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var item in entities)
            {
                ReleaseEntity(item);
            }

            entities.Clear();
        }

        private void TryCreateEntities()
        {
            if (creates.Count > 0)
            {
                foreach (var item in creates)
                {
                    Create(item);
                }

                creates.Clear();
            }
        }

        private void TryReleaseEntities()
        {
            if (releases.Count > 0)
            {
                foreach (var item in releases)
                {
                    Release(item);
                }

                releases.Clear();
            }
        }

        public void Tick()
        {
            ElapseFrame++;
            CheckEntities();
            OnTick();
            foreach (var item in entities)
            {
                item.Tick();
            }
        }

        public void CheckEntities()
        {
            TryReleaseEntities();
            TryCreateEntities();
        }

        public void Rollback()
        {
            ElapseFrame--;
            OnRollback();
            foreach (var item in entities)
            {
                if (item.ElapseFrame < 0)
                {
                    PutRelease(item.InstId);
                }
                else
                {
                    item.Rollback();
                }
            }

            TryReleaseEntities();
        }

        public void Update(float deltaTime)
        {
            ElapseTime += deltaTime;
            OnUpdate(deltaTime);
            foreach (var item in entities)
            {
                item.Update(deltaTime);
            }
        }

        public void Dispose()
        {
            foreach (var item in entities)
            {
                item.Dispose();
            }
            
            if (null != pool)
            {
                foreach (var item in pool)
                {
                    item.Dispose();
                }

                pool = null;
            }

            OnDispose();

            entities = null;
            creates = null;
            releases = null;
            createEvent = null;
            releaseEvent = null;
        }

        protected virtual void OnInitial()
        {
        }

        protected virtual void OnTick()
        {
        }

        protected virtual void OnRollback()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected virtual void OnDispose()
        {
        }
    }
}