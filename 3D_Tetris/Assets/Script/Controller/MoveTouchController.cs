using System;
using System.Collections.Generic;
using Script.Controller;
using UnityEngine;

public class MoveTouchController : MonoBehaviour
{
    public enum StateTouch
    {
        none,
        waitInOnePoint,
        open,
        swipe,
    }
    
    public event Action<StateTouch> onStateChanged;

    public StateTouch currentState => _state;

    [SerializeField] private float _timeOpen;
    
    private StateTouch _state = StateTouch.none;

    private float _timer = 0;

    private Vector2 _lastPosition;

    private Vector2 _openPoint;

    private TetrisFSM _fsm;
    // Start is called before the first frame update
    void Start()
    {
        _fsm = RealizationBox.Instance.FSM;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if(currentState == StateTouch.open && Input.touches.Length != 1)
            OnBreak();
            
        if (_fsm.GetCurrentState() != TetrisState.WaitInfluence)
            return;
        
        if (Input.touches.Length != 1)
        {
            if(_state != StateTouch.none) 
                OnBreak();
            return;
        }

        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            OnBeganTouch();
            return;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            OnBreak();
            return;
        }

        if (_state == StateTouch.waitInOnePoint)
        {
            if (Vector2.Distance(_lastPosition, touch.position) < Screen.width * 0.05f)
                OnWaitInOnePoint();
            else
                OnNotWaitInOnePoint();
        }
    }

    private void OnBeganTouch()
    {
        SetState(StateTouch.waitInOnePoint);
        
        _timer = 0;
        _lastPosition = Input.GetTouch(0).position;
    }

    private void OnWaitInOnePoint()
    {
        _timer += Time.deltaTime;

        if (_timer > _timeOpen)
            OnTouchOpen();
    }

    private void OnNotWaitInOnePoint()
    {
        SetState(StateTouch.swipe);
    }

    private void OnTouchOpen()
    {
        SetState(StateTouch.open);
        _fsm.SetNewState(TetrisState.MoveMode);
    }

    private void OnBreak()
    {
        SetState(StateTouch.none);
    }

    private void SetState(StateTouch newState)
    {
        _state = newState;
        onStateChanged?.Invoke(_state);
    }
}
