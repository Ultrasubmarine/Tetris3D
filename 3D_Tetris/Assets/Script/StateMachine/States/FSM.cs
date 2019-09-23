using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : IFSM<States> {
	
	// TODO DO DO
	public States CurrentState { get; set; }
	
	IFSM<States> _ifsmImplementation;
	public event Action<States, States> SetNewState {
		add { _ifsmImplementation.SetNewState += value; }
		remove { _ifsmImplementation.SetNewState -= value; }
	}


	public void SetState(States newState) {
		States last = CurrentState;
		CurrentState = newState;
		
//		SetNewState.Invoke( last, newState);
	}
}
