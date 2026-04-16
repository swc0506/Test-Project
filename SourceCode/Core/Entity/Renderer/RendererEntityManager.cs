using System;

namespace Core.Entity
{
    public abstract class RendererEntityManager<T> : EntityManager<T>, IRendererEntityManager where T : RendererEntity
    {
        private GameObjectPool goPool;
        public GameObjectPool GoPool
        {
            get
            {
                if (null == goPool)
                {
                    goPool = CreateGameObjectPool();
                }

                return goPool;
            }
        }

        public Action<RendererEntity> SetSkinEvent { get; set; }


        protected virtual GameObjectPool CreateGameObjectPool()
        {
            IGameObjectProvider provider = new AssetPackageProvider(GetPackageName());
            return new GameObjectPool(provider);
        }

        protected abstract string GetPackageName();


        protected override void OnNewEntity(T entity)
        {
            entity.renderer = entity.AddComponent<EntityRendererComponent>();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (null != goPool)
            {
                goPool.Dispose();
                goPool = null;
            }
        }
    }
}