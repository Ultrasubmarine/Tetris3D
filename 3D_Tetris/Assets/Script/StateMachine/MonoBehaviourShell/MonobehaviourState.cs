using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script.StateMachine.MonoBehaviourShell
{
	public abstract class MonobehaviourState<T> : IState<T>
	{
		protected T _myState;

//		[SerializeField] protected UnityEvent _OnEnter;
//		[SerializeField] protected UnityEvent _OnExit;
		private IState<T> _StateImplementation;

		public Action AdditionalActions;
		public T GetState () { return _myState; }

		public abstract void Enter(T last);

		public abstract void Exit(T last);
	}
}
