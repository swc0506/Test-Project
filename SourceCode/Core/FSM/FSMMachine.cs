using System.Collections.Generic;

namespace Core
{
    public class FSMStateArgs
    {
    }

    public class FSMMachine
    {
        private bool checkTransition;
        private Dictionary<int, FSMState> stateMap = new Dictionary<int, FSMState>();
        public FSMState CurrState { get; private set; }

        public FSMMachine(bool checkTransition)
        {
            this.checkTransition = checkTransition;
        }

        public FSMMachine() : this(true)
        {
        }

        public FSMState GetState(int stateId)
        {
            if (stateMap.TryGetValue(stateId, out var state))
            {
                return state;
            }

            return null;
        }

        public bool AddState(FSMState state)
        {
            if (stateMap.ContainsKey(state.StateId))
            {
                return false;
            }

            stateMap.Add(state.StateId, state);
            return true;
        }

        public bool RemoveState(int stateId)
        {
            return stateMap.Remove(stateId);
        }

        private void EnterState(FSMState state, FSMStateArgs args)
        {
            if (null != CurrState)
            {
                CurrState.Exit();
            }

            CurrState = state;
            CurrState.Enter(args);
        }

        /// <summary>
        /// 设置默认状态
        /// </summary>
        /// <param name="stateId"></param>
        public void SetDefault(int stateId)
        {
            if (stateMap.TryGetValue(stateId, out var state))
            {
                EnterState(state, null);
            }
        }

        /// <summary>
        /// 转换状态
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Transition(int trans, FSMStateArgs args)
        {
            if (null != CurrState)
            {
                int stateId = CurrState.GetTransitionStateId(trans);
                if (stateMap.TryGetValue(stateId, out var state))
                {
                    EnterState(state, args);
                    return true;
                }
            }

            return false;
        }

        public void CheckTransition()
        {
            if (checkTransition && null != CurrState)
            {
                int trans = CurrState.CheckTransition(out FSMStateArgs args);
                if (trans >= 0)
                {
                    Transition(trans, args);
                }
            }
        }

        public void Update(float deltaTime)
        {
            if (null != CurrState)
            {
                CurrState.Update(deltaTime);
            }
        }
    }
}