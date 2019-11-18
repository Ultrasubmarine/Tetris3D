using Helper.Patterns.FSM;
using UnityEngine;

public class CollectionState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;

    public CollectionState()
    {
        _matrix = RealizationBox.Instance.matrix;
    }

    public override void Enter(TetrisState last)
    {
        if (_matrix.CollectLayers())
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        else
            _FSM.SetNewState(TetrisState.WinCheck);
        
        base.Enter(last);
    }

    public override void Exit(TetrisState last)
    {
    }
}