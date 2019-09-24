using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IFSM<T>
{
	T CurrentState { get; set; }
	
	event Action<T,T> StateChanged;
	
	void InitFSM();
	void SetNewState(T newState);

}

