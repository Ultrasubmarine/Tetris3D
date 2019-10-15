using System;

public interface IState<T> 
{
	T GetState();
	void Enter(T last);
	void Exit( T last) ;
	
}
