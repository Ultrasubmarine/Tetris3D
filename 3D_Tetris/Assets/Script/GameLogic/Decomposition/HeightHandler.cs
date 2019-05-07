﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntegerExtension;

public class HeightHandler : MonoBehaviour {

    PlaneMatrix _matrix;

    [SerializeField, Space(20)] int _LimitHeight;
    [SerializeField] int _CurrentHeight;

    public int LimitHeight{ get { return _LimitHeight; } }
    public int CurrentHeight { get { return _CurrentHeight; } }

    private void Start() {
        _matrix = PlaneMatrix.Instance;
        _matrix.SetLimitHeight(_LimitHeight);
    }
    public bool CheckLimit() {

        CheckHeight();
        return OutOfLimitHeight();
    }

    public void CheckHeight() {

        _CurrentHeight = 0;
        int check;

        for (int x = 0; x < _matrix.Wight && !OutOfLimitHeight(); x++) {
            for (int z = 0; z < _matrix.Wight && !OutOfLimitHeight(); z++) {

               check = _matrix.MinHeightInCoordinates(x, z);
                if(check > _CurrentHeight) {
                    _CurrentHeight = check;           
                }
            }
        }

        Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, _CurrentHeight +1);
    }

    private bool OutOfLimitHeight( ) {

        if (_CurrentHeight <= LimitHeight)
            return false;
        return true;
    }
}