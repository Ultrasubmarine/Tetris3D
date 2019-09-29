using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.StateMachine.MonoBehaviourShell
{
	public class FSM<T> : MonoBehaviour,  IFSM<T> {
	
		// TODO DO DO
		protected Dictionary<T, IState<T>> _statesDictionary;
		
		protected T _last;
		protected T _current;
		
		public T GetCurrentState() { return _current;}

		public virtual void SetNewState(T newState)
		{
			throw new NotImplementedException();
		}

	}
}
