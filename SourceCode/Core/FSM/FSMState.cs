using System.Collections.Generic;

namespace Core
{
    public abstract class FSMState
    {
        public int StateId { get; private set; }
        private Dictionary<int, int> transMap = new Dictionary<int, int>();

        public void Initial(int stateId)
        {
            this.StateId = stateId;
            OnInitial();
        }

        protected virtual void OnInitial()
        {
        }

        public bool AddTransition(int trans, int stateId)
        {
            if (transMap.ContainsKey(trans))
            {
                return false;
            }

            transMap.Add(trans, stateId);
            return true;
        }

        public bool RemoveTransition(int trans)
        {
            return transMap.Remove(trans);
        }

        public int GetTransitionStateId(int trans)
        {
            if (transMap.TryGetValue(trans, out var stateId))
            {
                return stateId;
            }

            return 0;
        }

        /// <summary>
        /// 进入该状态调用
        /// </summary>
        /// <param name="args"></param>
        public abstract void Enter(FSMStateArgs args);

        /// <summary>
        /// 退出该状态调用
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 检测状态转换
        /// </summary>
        /// <returns></returns>
        public abstract int CheckTransition(out FSMStateArgs args);

        /// <summary>
        ///在该状态每渲染帧都调用 
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void Update(float deltaTime);
    }
}