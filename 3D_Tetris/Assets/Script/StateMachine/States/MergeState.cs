using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class MergeState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    
    public MergeState()
    {
        _matrix = RealizationBox.Instance.matrix;
        _heightHandler = RealizationBox.Instance.haightHandler;
    }

    public override void Enter(TetrisState last)
    {
        _matrix.BindToMatrix(ElementData.newElement);
        ElementData.MergeNewElement();
        
        base.Enter(last);
        if(!_heightHandler.CheckOutOfLimit())
            _FSM.SetNewState(TetrisState.Collection);
        else
            _FSM.SetNewState(TetrisState.LoseGame);
    }

    public override void Exit(TetrisState last)
    {
    }
}