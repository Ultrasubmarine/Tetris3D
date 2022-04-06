using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helper.Patterns.FSM
{
    public abstract class AbstractFSM<T> : MonoBehaviour where T : Enum
    {
        public Action<T> onStateChange;
        
        protected Dictionary<T, AbstractState<T>> _statesDictionary;

        protected T _last;
        protected T _current;

        public T startState;
        
        public T GetCurrentState()
        {
            return _current;
        }

        private void Awake()
        {
            _statesDictionary = new Dictionary<T, AbstractState<T>>();
        }

        public abstract void StartFSM();

        public abstract void StartFSMFromCustomState(T state);
        
        public virtual void SetNewState(T newState)
        {
			_statesDictionary[_current].Exit(newState);

            _last = _current;
            _current = newState;
//
//         Debug.Log("new state " + newState);
            _statesDictionary[_current].Enter(_last);
            onStateChange?.Invoke(newState);
        }

        public void AddListener(T state, Action callMethod)
        {
            _statesDictionary[state].AdditionalActions += callMethod;
        }

        public void RemoveListener(T state, Action callMethod)
        {
            _statesDictionary[state].AdditionalActions -= callMethod;
        }
    }
}