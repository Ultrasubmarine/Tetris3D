using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachineListener : MonoBehaviour
{
	[SerializeField] EMachineState _State;
	[SerializeField] UnityEvent _Event;

	void Awake() {
		Messenger.AddListener(StateMachine.StateMachineKey + _State.ToString(), Invoke);
	}

	public void Broadcast() {
		Messenger.RemoveListener(StateMachine.StateMachineKey + _State.ToString(), Invoke);
	}

	public void Invoke() {
		_Event.Invoke();
	}
}
