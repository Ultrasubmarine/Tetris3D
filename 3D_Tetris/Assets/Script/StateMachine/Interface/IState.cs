using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IState<T>
{
	T GetState();
	void Enter(T last);
	void Exit( T last) ;
	
}
