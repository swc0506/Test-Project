using System;

namespace Core.Entity
{
    public interface IRendererEntityManager : IEntityManager
    {
        GameObjectPool GoPool { get; }
        Action<RendererEntity> SetSkinEvent { get; set; }
    }
}