using UnityEngine;
using UnityEngine.Events;

namespace Script.StateMachine.MonoBehaviourShell
{
	public class MonobehaviourState<T> : MonoBehaviour, IState<T>
	{
		protected T _myState;

		[SerializeField] protected UnityEvent _OnEnter;
		[SerializeField] protected UnityEvent _OnExit;

		public T State {
			get { return _myState; }
			set { Debug.LogError("You try change state in IState object"); }
		}

		public virtual void Enter(IState<T> last, Element element = null) {
			_OnEnter.Invoke();
		}

		public virtual void Exit(IState<T> last, Element element = null) {
			_OnExit.Invoke();
		}
	}
}
