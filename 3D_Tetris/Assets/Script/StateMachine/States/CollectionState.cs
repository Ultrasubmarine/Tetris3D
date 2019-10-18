using Helper.Patterns.FSM;
using UnityEngine;

public class CollectionState : AbstractState<TetrisState>
{
    private PlaneMatrix _matrix;
    
    public CollectionState()
    {
        _matrix = RealizationBox.Instance.Matrix();
    }
    
    public override void Enter(TetrisState last)
    {
        if (_matrix.CollectLayers())
        {
            Debug.Log(" Собрали коллекцию");
          _FSM.SetNewState(TetrisState.AllElementsDrop);
        }
        else
        {
            Debug.Log(" Никаких коллекций коллекцию");
            _FSM.SetNewState(TetrisState.WinCheck);
        }
    }

    public override void Exit(TetrisState last) { }
}
