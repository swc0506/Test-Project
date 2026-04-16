using System;
using System.Collections.Generic;

namespace Core
{
    public enum StageState
    {
        Idle,
        Preload,
        Active,
        Inactive,
        Dispose
    }

    public abstract class Stage
    {
        public StageState State { get; private set; }
        private List<IStageBehaviour> behaviours = new List<IStageBehaviour>();
        private List<IUpdateable> updates = new List<IUpdateable>();

        public int TotalCount { get; protected set; }

        private int completedCount;

        public int CompletedCount
        {
            get { return completedCount; }
            set
            {
                if (completedCount != value)
                {
                    completedCount = value;
                    OnRefreshCompletedCount();
                }
            }
        }

        public void Initial()
        {
            State = StageState.Idle;
            OnInitial();
        }

        public void Preload()
        {
            State = StageState.Preload;
            TotalCount = 0;
            completedCount = 0;
            OnPreload();
        }

        private void Enter()
        {
            OnEnter();
            State = StageState.Active;
            foreach (var item in behaviours)
            {
                item.Start();
            }

            OnAfterEnter();
        }

        public void Update(float deltaTime)
        {
            if (State == StageState.Preload && completedCount == TotalCount)
            {
                Enter();
            }

            OnUpdate(deltaTime);
            foreach (var item in updates)
            {
                item.Update(deltaTime);
            }
        }

        private void Exit()
        {
            State = StageState.Inactive;
            OnExit();
            foreach (var item in behaviours)
            {
                item.Stop();
            }

            OnAfterExit();
        }

        public void Dispose()
        {
            if (State == StageState.Active)
            {
                Exit();
            }

            foreach (var item in behaviours)
            {
                item.Dispose();
            }

            OnDispose();
            behaviours = null;
            updates = null;
        }

        private int FindBehaviourIndex(Type type)
        {
            for (int i = 0, length = behaviours.Count; i < length; i++)
            {
                if (behaviours[i].GetType() == type)
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddBehaviour(Type type)
        {
            int index = FindBehaviourIndex(type);
            if (index >= 0)
            {
                return;
            }

            IStageBehaviour behaviour = (IStageBehaviour)Activator.CreateInstance(type);
            behaviours.Add(behaviour);
            if (behaviour is IUpdateable update)
            {
                updates.Add(update);
            }

            behaviour.Initial(this, behaviour);
            if (State == StageState.Active)
            {
                behaviour.Start();
            }
        }

        public void AddBehaviour<T>() where T : IStageBehaviour
        {
            AddBehaviour(typeof(T));
        }


        public void RemoveBehaviour(Type type)
        {
            int index = FindBehaviourIndex(type);
            if (index < 0)
            {
                return;
            }

            IStageBehaviour behaviour = behaviours[index];
            if (behaviour is IUpdateable update)
            {
                updates.Remove(update);
            }

            behaviours.RemoveAt(index);

            if (State == StageState.Active)
            {
                behaviour.Stop();
            }

            behaviour.Dispose();
        }

        public void RemoveBehaviour<T>() where T : IStageBehaviour
        {
            RemoveBehaviour(typeof(T));
        }

        public IStageBehaviour GetBehaviour(string name)
        {
            for (int i = 0, length = behaviours.Count; i < length; i++)
            {
                if (behaviours[i].GetType().Name == name)
                {
                    return behaviours[i];
                }
            }

            return null;
        }

        public IStageBehaviour GetBehaviour(Type type)
        {
            for (int i = 0, length = behaviours.Count; i < length; i++)
            {
                if (behaviours[i].GetType() == type)
                {
                    return behaviours[i];
                }
            }

            return null;
        }

        public T GetBehaviour<T>() where T : IStageBehaviour
        {
            return (T)GetBehaviour(typeof(T));
        }

        protected virtual void OnRefreshCompletedCount()
        {
        }

        protected virtual void OnInitial()
        {
        }

        protected virtual void OnPreload()
        {
        }

        protected abstract void OnEnter();

        protected virtual void OnAfterEnter()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected abstract void OnExit();

        protected virtual void OnAfterExit()
        {
        }

        protected virtual void OnDispose()
        {
        }
    }
}