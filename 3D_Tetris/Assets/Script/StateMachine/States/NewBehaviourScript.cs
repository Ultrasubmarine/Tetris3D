using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
	A,
	B
}
public class NewBehaviourScript : MonoBehaviour, IState<States> {
	
	IState<States> _stateImplementation ;

	void Awake() {
		State = States.A;
	}

	public States State {
		get { return _stateImplementation.State; }
		set { Debug.LogError(" you try change state in IState object");}
	}

	public void Enter(IState<States> last, Element element = null) {
		Debug.Log("jj");
	}

	public void Exit(IState<States> last, Element element = null) {
		Debug.Log("jj");
	}
}
