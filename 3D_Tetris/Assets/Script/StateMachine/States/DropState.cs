using System.Collections;
using System.Collections.Generic;
using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class DropState : AbstractState<TetrisState>
{
    Generator _generator;
    ElementManager _elementManager;
    private PlaneMatrix _matrix;

    public DropState()
    {
        _myState = TetrisState.Drop;
        
        _elementManager = RealizationBox.Instance.ElementManager();
        _matrix = RealizationBox.Instance.Matrix();
    }

    public override void Enter(TetrisState last)
    {
        bool empty = _matrix.CheckEmptyPlaсe(ElementData.NewElement, new Vector3Int(0, -1, 0));
        if (empty)
        {
            _elementManager.StartDropElement();
            return;
        }
        
        _FSM.SetNewState( TetrisState.MergeElement);
    }

    public override void Exit(TetrisState last) {
    }

 
    
}
