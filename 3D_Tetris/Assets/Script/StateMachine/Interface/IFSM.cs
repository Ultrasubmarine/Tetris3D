using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IFSM<T>
{
	T GetCurrentState();

	void SetNewState(T newState);

	void AddListener(T state, )
}

