using System;

namespace Core
{
    public class AppStage : Stage
    {
        public static AppStage Instance { get; private set; }
        public Stage CurrStage { get; private set; }
        private Type runStageType;

        public static void Startup()
        {
            Instance = new AppStage();
            Instance.Initial();
            Instance.Preload();
        }

        public static void Shutdown()
        {
            if (null != Instance)
            {
                Instance.Dispose();
                Instance = null;
            }
        }

        protected override void OnEnter()
        {
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (null != runStageType)
            {
                RunImmediate(runStageType);
                runStageType = null;
            }

            if (null != CurrStage)
            {
                CurrStage.Update(deltaTime);
            }
        }

        protected override void OnExit()
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (null != CurrStage)
            {
                CurrStage.Dispose();
            }

            CurrStage = null;
        }

        public void Run(Type stageType)
        {
            runStageType = stageType;
        }

        public void Run<T>() where T : Stage
        {
            Run(typeof(T));
        }

        public void RunImmediate(Type type)
        {
            if (null != CurrStage)
            {
                CurrStage.Dispose();
            }

            //构造新场景
            CurrStage = Activator.CreateInstance(type) as Stage;
            CurrStage.Initial();
            CurrStage.Preload();
        }

        public void RunImmediate<T>() where T : Stage
        {
            RunImmediate(typeof(T));
        }
    }
}