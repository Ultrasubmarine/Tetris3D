using Helper.Patterns.FSM;
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
	Collection,
	DropAllElements,
	WinCheck,

}

public class TetrisFSM  :  AbstractFSM<TetrisState>
{
	private void Start()
	{
		AbstractState<TetrisState>.SetMainFSM(this);
		
		_statesDictionary.Add( TetrisState.GenerateElement, new GenerationState() );
		_statesDictionary.Add( TetrisState.Drop, new DropState());
		_statesDictionary.Add( TetrisState.MergeElement, new MergeState());

		
		Debug.Log(" Load fms");
		

//		Invoke( "StartFSM", 1.0f);
	}
	
	public override void StartFSM() {
		_current = TetrisState.GenerateElement;
		_statesDictionary[_current].Enter(TetrisState.GenerateElement);
	}
	
}
