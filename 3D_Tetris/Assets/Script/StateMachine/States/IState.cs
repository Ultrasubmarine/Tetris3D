using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
	T State { get; set; }
	void Enter(IState<T> last, Element element = null);
	void Exit( IState<T> last, Element element = null) ;
	
}
