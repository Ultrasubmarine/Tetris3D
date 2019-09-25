using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.StateMachine.MonoBehaviourShell
{
	public class MonoBehaviourFSM<T> : IFSM<T> {
	
		// TODO DO DO
		Dictionary<T, IState<T>> _statesDictionary;
	
		public T CurrentState {
			get { return _current;}
			set { Debug.LogError("You try change IFSM.CurrentState in object to: " + value);}
		}

		private T _last;
		private T _current;

		public event Action<T, T> StateChanged;

		public virtual void InitFSM()
		{
		}

		public virtual void SetNewState(TetrisState newState) {
		
			_statesDictionary[CurrentState].Exit( newState);
			States last = CurrentState;
			CurrentState = newState;

			StateChanged.Invoke(last, newState);
		}
	
	}
}
