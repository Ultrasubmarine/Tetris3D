using Helper.Patterns.FSM;
using Script.StateMachine.States;
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
    AllElementsDrop,
    WinCheck,
    
    LoseGame,
    WinGame,
}

public class TetrisFSM : AbstractFSM<TetrisState>
{
    private void Start()
    {
        AbstractState<TetrisState>.SetMainFSM(this);

        _statesDictionary.Add(TetrisState.GenerateElement, new GenerationState());

        _statesDictionary.Add(TetrisState.Drop, new DropState());

        _statesDictionary.Add(TetrisState.MergeElement, new MergeState());
        _statesDictionary.Add(TetrisState.Collection, new CollectionState());
        _statesDictionary.Add(TetrisState.WinCheck, new WinCheckState());
        _statesDictionary.Add(TetrisState.AllElementsDrop, new AllElementsDropState());
        
        _statesDictionary.Add(TetrisState.WaitInfluence, new WaitInfluenceState());
        _statesDictionary.Add(TetrisState.EndInfluence, new EndInfluenceState());
        _statesDictionary.Add(TetrisState.Move, new MoveState());

        _statesDictionary.Add(TetrisState.LoseGame, new LoseGameState());
//		Invoke( "StartFSM", 1.0f);
    }

    public override void StartFSM()
    {
        _current = TetrisState.GenerateElement;
        _statesDictionary[_current].Enter(TetrisState.GenerateElement);
    }
}