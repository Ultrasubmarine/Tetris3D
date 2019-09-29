using UnityEngine;
using UnityEngine.Events;

namespace Script.StateMachine.MonoBehaviourShell
{
	public class MonobehaviourState<T> : MonoBehaviour, IState<T>
	{
		protected T _myState;

		[SerializeField] protected UnityEvent _OnEnter;
		[SerializeField] protected UnityEvent _OnExit;
		private IState<T> _StateImplementation;

		public T GetState () { return _myState; }

		public virtual void Enter(T last) {
			_OnEnter.Invoke();
		}

		public virtual void Exit(T last) {
			_OnExit.Invoke();
		}
	}
}
