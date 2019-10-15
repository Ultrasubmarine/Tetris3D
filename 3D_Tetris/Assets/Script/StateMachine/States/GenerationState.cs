using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Script.StateMachine.MonoBehaviourShell;
using UnityEngine;

public class GenerationState : MonobehaviourState<TetrisState>
{
	FSM<TetrisState> myFSM;
	
	Generator _generator;
	ElementManager _elementManager;

	public GenerationState() {
		_generator = RealizationBox.Instance.ElementGenerator();
		_elementManager = RealizationBox.Instance.ElementManager();
	}

	public override void Enter(TetrisState last) {

		Debug.Log("Generate new ONN");
		var element = _generator.GenerationNewElement( _elementManager.transform);
		ElementManager.NewElement = element;
		
		
		Debug.Log("Generate new element");
//		_OnEnter.Invoke();
		
		myFSM.SetNewState( TetrisState.Drop);
	}

	public override void Exit(TetrisState last) {
//		_OnExit.Invoke();
	}
	
}
