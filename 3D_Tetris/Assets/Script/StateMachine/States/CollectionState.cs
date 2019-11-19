using Helper.Patterns.FSM;
using UnityEngine;

public class CollectionState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;

    public CollectionState()
    {
        _matrix = RealizationBox.Instance.matrix;
        _heightHandler = RealizationBox.Instance.haightHandler;
    }

    public override void Enter(TetrisState last)
    {
        if (_matrix.CollectLayers())
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        else
            _FSM.SetNewState(TetrisState.WinCheck);
        
        _heightHandler.CalculateHeight();
        base.Enter(last);
    }

    public override void Exit(TetrisState last)
    {
    }
}