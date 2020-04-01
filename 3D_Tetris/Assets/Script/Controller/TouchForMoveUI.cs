using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchForMoveUI : MonoBehaviour
{
    [SerializeField] private TouchForMove _touchForMove;

    [SerializeField] private float _radius = Screen.width * 0.1f;

    [SerializeField] private List<Image> _images;

    
    private void Awake()
    {
        _touchForMove.onStateChanged += TouchStateChange;
    }

    private void TouchStateChange( TouchForMove.StateTouch lastState, TouchForMove.StateTouch newState )
    {

        switch (newState)
        {
            case TouchForMove.StateTouch.timeOpen:
                break;    
        }
    }

    private void CreatePoints()
    {
        var mainPoint = Input.GetTouch(0).position;
        
    //    angle 
        
    }
}
