using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBroadcaster : MonoBehaviour
{
	[SerializeField] EMachineState _State;
	string strBroadcast;

	void Awake() {
		strBroadcast = StateMachine.StateMachineKey + _State.ToString();
	}

	public void Broadcast() {
			Messenger.Broadcast(strBroadcast);
	}
}
