using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IFSM<T>
{
	T CurrentState { get; set; }
	
	event Action<T,T> SetNewState;
	void SetState(T newState);
}

