using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveTouchController : MonoBehaviour
{
    public event Action onMoved;
    public event Action<StateTouch, StateTouch> onStateChanged; // 1-last, 2-new

    public StateTouch currentState => _state;
    
    
    [SerializeField] private float _timeOpen;

    
    private StateTouch _state = StateTouch.none;

    private float _timer = 0;

    private Vector2 _lastPosition;

    private Vector2 _openPoint;
    
    // Start is called before the first frame update
    void Start()
    {

    }


    public void ListenPoints(List<MovePointUi> points)
    {
        foreach (var point in points)
        {
            point.onPointEnter += OnMovedInPoint;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
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
        Debug.Log("Swipe");
    }

    private void OnTouchOpen()
    {
        SetState(StateTouch.timeOpen);
        Debug.Log("Open");
    }

    private void OnBreak()
    {
        SetState(StateTouch.none);
    }

    private void OnMovedInPoint()
    {
        if (currentState == StateTouch.timeOpen || currentState == StateTouch.movedInPoint)
        {
            SetState(StateTouch.movedInPoint);
            onMoved?.Invoke();
        }
    }
    
    private void SetState(StateTouch newState)
    {
        onStateChanged?.Invoke(_state, newState);
        _state = newState;
    }
    
    public enum StateTouch
    {
        none,
        waitInOnePoint,
        timeOpen,
        swipe,
        movedInPoint,
    }
}
