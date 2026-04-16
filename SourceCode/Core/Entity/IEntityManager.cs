using System;

namespace Core.Entity
{
    public interface IEntityManager : IDisposable
    {
        int ElapseFrame { get; }

        float ElapseTime { get; }

        int Count { get; }
        
        Entity Create(CreateArgs args);

        void Release(int instId);


        void PutCreate(CreateArgs args);

        void PutRelease(int instId);

        void ReleaseAll();

        void Tick();

        void Rollback();

        void Update(float deltaTime);
    }
}