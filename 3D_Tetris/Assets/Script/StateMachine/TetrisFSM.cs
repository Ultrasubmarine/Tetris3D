﻿using System;
using System.Collections.Generic;
using Script.StateMachine.MonoBehaviourShell;
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

public class TetrisFSM  :  FSM<TetrisState>
{
	private void Awake()
	{
		_statesDictionary = new Dictionary<TetrisState, IState<TetrisState>>();
		
		_statesDictionary.Add( TetrisState.GenerateElement, new GenerationState() );
		_statesDictionary.Add( TetrisState.Drop, new GenerationState() );
	}

	public virtual void SetNewState(TetrisState newState)
	{
		_statesDictionary[_current].Exit( newState);
		
		_last = _current;
		_current = newState;

		_statesDictionary[_current].Enter( _last);
	}

}