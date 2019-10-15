using System;

namespace Helper.Patterns.FSM
{
	public abstract class AbstractState<T> where T: Enum
	{
		protected T _myState;
		public Action AdditionalActions;

		public abstract void Enter(T last);

		public abstract void Exit(T last);
	}
}
