using System;
using Helper.Patterns.FSM;
using Script.StateMachine.States;

public enum TetrisState
{
    GenerateElement,

    Drop,
    Move,

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
    public event Action OnStart;
    
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
        _statesDictionary.Add(TetrisState.WinGame, new WinState());
        
        RealizationBox.Instance.gameCamera.onFirstAnimationEnd += StartFSM;
//		Invoke( "StartFSM", 1.0f);
    }

    public override void StartFSM()
    {
        OnStart?.Invoke();
        _current = TetrisState.GenerateElement;
        _statesDictionary[_current].Enter(TetrisState.GenerateElement);
    }
}