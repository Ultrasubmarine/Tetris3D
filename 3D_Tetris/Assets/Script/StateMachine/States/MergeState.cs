using Helper.Patterns.FSM;
using Script.GameLogic.Bomb;
using Script.GameLogic.TetrisElement;
using Script.Projections;
using UnityEngine;

public class MergeState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private Generator _generator;
    private ProjectionLineManager _projLineManager;
    private BombsManager _bombsManager;
    
    public MergeState()
    {
        _matrix = RealizationBox.Instance.matrix;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _generator = RealizationBox.Instance.generator;
        _projLineManager = RealizationBox.Instance.projectionLineManager;
        _bombsManager = RealizationBox.Instance.bombsManager;
    }

    public override void Enter(TetrisState last)
    {
        _matrix.BindToMatrix(ElementData.Instance.newElement);
        ElementData.Instance.MergeNewElement();
      //  _generator.DestroyOldDuplicate();
     //   _projLineManager.UpdatePickableProjections();
        _projLineManager.Clear();
        
        RealizationBox.Instance.joystick.gameObject.SetActive(false);
        base.Enter(last);
        if(_bombsManager.BoomBombs())
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        else if(!_heightHandler.CheckOutOfLimit())
            _FSM.SetNewState(TetrisState.Collection);
        else
            _FSM.SetNewState(TetrisState.LoseGame);
    }

    public override void Exit(TetrisState last)
    {
    }
}