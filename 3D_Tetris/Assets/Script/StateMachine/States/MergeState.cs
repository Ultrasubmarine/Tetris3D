using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using Script.Projections;
using UnityEngine;

public class MergeState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private Generator _generator;
    private ProjectionLineManager _projLineManager;
    
    public MergeState()
    {
        _matrix = RealizationBox.Instance.matrix;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _generator = RealizationBox.Instance.generator;
        _projLineManager = RealizationBox.Instance.projectionLineManager;
    }

    public override void Enter(TetrisState last)
    {
        _matrix.BindToMatrix(ElementData.newElement);
        ElementData.MergeNewElement();
        _generator.DestroyOldDuplicate();
        _projLineManager.UpdatePickableProjections();
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