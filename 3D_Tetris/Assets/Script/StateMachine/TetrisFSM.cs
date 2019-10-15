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
	
}

public class TetrisFSM  :  AbstractFSM<TetrisState>
{
	private void Start()
	{
		_statesDictionary.Add( TetrisState.GenerateElement, new GenerationState() );
		_statesDictionary.Add( TetrisState.Drop, new GenerationState() );
		
		Debug.Log(" Load fms");
		Invoke( "StartFSM", 1.0f);
	}
	
	public override void StartFSM() {
		_current = TetrisState.GenerateElement;
		_statesDictionary[_current].Enter(TetrisState.GenerateElement);
	}
	
}
