using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TetrisState
{
	Empty,
	GenerateElement,
	
	Drop,
	Move,
	Turn,
	
	WaitInfluence,
	EndInfluence,
	
	MergeElement,
	
}
public class FSM : IFSM<TetrisState> {
	
	// TODO DO DO
	Dictionary<TetrisState, IState<TetrisState>> _statesDictionary;
	
	public TetrisState CurrentState {
		get;
		set;
	}

	private TetrisState _last;
	private TetrisState _current;

	public event Action<TetrisState, TetrisState> StateChanged;

	public void InitFSM()
	{
	}

	public void SetNewState(TetrisState newState) {
		
		_statesDictionary[CurrentState ].Exit();
		States last = CurrentState;
		CurrentState = newState;

		StateChanged.Invoke(last, newState);
	}
	
}
