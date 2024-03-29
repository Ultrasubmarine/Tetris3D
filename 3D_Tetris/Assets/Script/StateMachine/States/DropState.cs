﻿using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class DropState : AbstractState<TetrisState>
{
    private Generator _generator;
    private ElementDropper _elementDropper;
    private PlaneMatrix _matrix;
    private ElementData _elementData;
    
    public DropState()
    {
        _myState = TetrisState.Drop;

        _elementDropper = RealizationBox.Instance.elementDropper;
        _elementData = ElementData.Instance;
        _matrix = RealizationBox.Instance.matrix;
    }

    public override void Enter(TetrisState last)
    {
        base.Enter(last);
        
        if (last != TetrisState.WaitInfluence && last != TetrisState.EndInfluence && last != TetrisState.GenerateElement && last != TetrisState.BigBombGenegation)
        {
            InfluenceData.delayedDrop = true;
            return;
        }
        
        if (RealizationBox.Instance.bombsManager.bigBombFalling &&
            _elementData.newElement.blocks[0]._coordinates.y - 1 <= RealizationBox.Instance.haightHandler.currentHeight)
        {
            _FSM.SetNewState(TetrisState.MergeElement);
            return;
        }
        
        var empty = _matrix.CheckEmptyPlaсe(_elementData.newElement, new Vector3Int(0, -1, 0));
        if (empty && !_elementData.newElement.isFreeze)
        {
          
            _elementDropper.StartDropElement();
            
            _FSM.SetNewState(TetrisState.WaitInfluence);
            return;
        }
        
        if (_elementDropper.WaitMerge())
        {
            _FSM.SetNewState(TetrisState.WaitInfluence);
            return;
        }

        _FSM.SetNewState(TetrisState.MergeElement);
    }

    public override void Exit(TetrisState last)
    {
    }
}