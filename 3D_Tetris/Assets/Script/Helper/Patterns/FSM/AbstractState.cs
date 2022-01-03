using System;

namespace Helper.Patterns.FSM
{
    public abstract class AbstractState<T> where T : Enum
    {
        protected static AbstractFSM<T> _FSM;

        protected T _myState;
        public event Action AdditionalActions;

        public virtual void Enter(T last)
        {
            AdditionalActions?.Invoke();
        }

        public abstract void Exit(T last);

        public static void SetMainFSM(AbstractFSM<T> fsm)
        {
            _FSM = fsm;
        }
    }
}