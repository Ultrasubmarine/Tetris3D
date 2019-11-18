﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IntegerExtension;
using UnityEngine.Serialization;

public class HeightHandler : MonoBehaviour
{
    // DELETE
//    [FormerlySerializedAs("machine")] [SerializeField] StateMachine _Machine;
    //
    private PlaneMatrix _matrix;

    [SerializeField] [Space(20)] private int _LimitHeight;
    [SerializeField] private int _CurrentHeight;

    public int LimitHeight => _LimitHeight;
    public int CurrentHeight => _CurrentHeight;

    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _matrix.SetLimitHeight(_LimitHeight);

//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.Merge, ChangeStateOutOfHeights);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, CheckHeight);
    }

    private void OnDestroy()
    {
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Merge, ChangeStateOutOfHeights);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, CheckHeight);
    }

    public void ChangeStateOutOfHeights()
    {
        if (CheckOutOfLimit())
        {
//            _Machine.ChangeState(EMachineState.End);
            Debug.Log("END GAME");
        }
        else
        {
//            _Machine.ChangeState(EMachineState.Collection);
        }
    }

    public bool CheckOutOfLimit()
    {
        CheckHeight();
        return OutOfLimitHeight();
    }

    public void CheckHeight()
    {
        _CurrentHeight = 0;
        int check;

        for (var x = 0; x < _matrix.wight && !OutOfLimitHeight(); x++)
        for (var z = 0; z < _matrix.wight && !OutOfLimitHeight(); z++)
        {
            check = _matrix.MinHeightInCoordinates(x, z);
            if (check > _CurrentHeight) _CurrentHeight = check;
        }

//        Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT.ToString(), _LimitHeight, _CurrentHeight +1);
    }

    private bool OutOfLimitHeight()
    {
        if (_CurrentHeight <= LimitHeight)
            return false;
        return true;
    }
}