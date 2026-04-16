using System;

namespace Core
{
    public abstract class StageBehaviour<T> : IStageBehaviour where T : StageBehaviour<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        public Stage Context { get; private set; }


        void IStageBehaviour.Initial(Stage context, IStageBehaviour inst)
        {
            instance = (T)inst;
            Context = context;
            OnInitial();
        }

        protected virtual void OnInitial()
        {
        }

        public abstract void Start();


        public abstract void Stop();

        void IStageBehaviour.Dispose()
        {
            OnDispose();
            instance = null;
            Context = null;
        }

        protected virtual void OnDispose()
        {
        }
    }
}