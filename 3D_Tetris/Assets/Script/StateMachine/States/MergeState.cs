using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class MergeState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private Generator _generator;
    
    public MergeState()
    {
        _matrix = RealizationBox.Instance.matrix;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _generator = RealizationBox.Instance.generator;
    }

    public override void Enter(TetrisState last)
    {
        _matrix.BindToMatrix(ElementData.newElement);
        ElementData.MergeNewElement();
        _generator.DestroyOldDuplicate();
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