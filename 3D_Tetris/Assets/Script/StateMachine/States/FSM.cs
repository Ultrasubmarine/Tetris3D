using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : IFSM<States> {
	
	// TODO DO DO
	
	public States CurrentState {
		get;
		set;
	}
	
	public event Action<States, States> StateChanged;

	public void SetState(States newState) {
		States last = CurrentState;
		CurrentState = newState;
		
//		SetNewState.Invoke( last, newState);
	}
}
