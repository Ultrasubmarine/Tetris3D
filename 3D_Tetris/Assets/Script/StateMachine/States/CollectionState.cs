using Helper.Patterns.FSM;
using Script.GameLogic.Bomb;
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
        _matrix.OnDestroyLayerEnd += OnCollectEnd;
        _matrix.CollectLayers();
        //  _heightHandler.CalculateHeight();
        base.Enter(last);
    }

    public void OnCollectEnd(bool isDestroy)
    {
        _matrix.OnDestroyLayerEnd -= OnCollectEnd;

        if (_FSM.GetCurrentState() == TetrisState.Restart)
            return;

        if (isDestroy)
        {
            //collect in previous collect state 
            if (RealizationBox.Instance.evilBoxManager.isOpenedBox)
            {
                _FSM.SetNewState(TetrisState.OpenEvilBox);
            }
            else
            {
                _FSM.SetNewState(TetrisState.AllElementsDrop);
            }
        }
        else
        {
            _heightHandler.CalculateHeight();
            _FSM.SetNewState(TetrisState.WinCheck);
        }
    }
    public override void Exit(TetrisState last)
    {
    }
}