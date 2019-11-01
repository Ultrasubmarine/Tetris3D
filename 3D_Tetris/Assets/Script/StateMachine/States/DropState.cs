using System.Collections;
using System.Collections.Generic;
using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class DropState : AbstractState<TetrisState>
{
    private Generator _generator;
    private ElementDropper _elementDropper;
    private PlaneMatrix _matrix;

    public DropState()
    {
        _myState = TetrisState.Drop;

        _elementDropper = RealizationBox.Instance.elementDropper;
        _matrix = RealizationBox.Instance.matrix;
    }

    public override void Enter(TetrisState last)
    {
        var empty = _matrix.CheckEmptyPlaсe(ElementData.NewElement, new Vector3Int(0, -1, 0));
        if (empty)
        {
            _elementDropper.StartDropElement();
            return;
        }

        _FSM.SetNewState(TetrisState.MergeElement);
    }

    public override void Exit(TetrisState last)
    {
    }
}