using System;
using System.Collections.Generic;
using UnityEngine;

public class SlowManager : MonoBehaviour
{
    public struct Slow
    {
        public Timer timer;
        public float slow;

        public Slow(Timer timer, float slow)
        {
            this.timer = timer;
            this.slow = slow;
        }
    }

    private MoveTouchController _moveTouchController;
    
    public float slow => _slowlerValue;

    public Action onUpdateValue;
    
    
    private List<Slow> _slowsList = new List<Slow>();

    private float _slowlerValue = 0;

    private Slow? MoveModeSlow;
    

    
    public void AddedSlow(float time, float value)
    {
        var timer = TimersKeeper.Schedule(time);
        var slow = new Slow(timer, value);

        timer.onStateChanged += (s) => 
        { 
            if (s == TimerState.Completed)
                OnDestroyTimer(slow);
        };
        
        _slowsList.Add(slow);
        CalculateSlow();
    }
    
    private void AddedMoveModeSlow(float time, float value)
    {
        var timer = TimersKeeper.Schedule(time);
        var slow = new Slow(timer, value);

        timer.onStateChanged += (s) =>
        {
            if (s == TimerState.Completed)
                OnDestroyMoveModeTimer();
        };
        MoveModeSlow = slow;
        
        CalculateSlow();
    }

    private void RemoveMoveModeSlow()
    {
        if (MoveModeSlow == null)
            return;
        
        MoveModeSlow.Value.timer.Cancel();
        MoveModeSlow = null;
        CalculateSlow();
    }

    private void Start()
    {
        _moveTouchController = RealizationBox.Instance.moveTouchController;
        _moveTouchController.onStateChanged += OnMoveTouchControllerStateChange;
    }

    private void OnDestroyTimer(Slow slowler)
    {
        _slowsList.Remove(slowler);
        CalculateSlow();
    }
    
    private void OnDestroyMoveModeTimer()
    {
        MoveModeSlow = null;
        CalculateSlow();
    }

    private void CalculateSlow()
    {
        _slowlerValue = 0;
        foreach (var slow in _slowsList)
        {
            _slowlerValue += slow.slow;
        }

        if (MoveModeSlow != null)
            _slowlerValue += MoveModeSlow.Value.slow;
        
        onUpdateValue?.Invoke();
    }

    private void OnMoveTouchControllerStateChange(MoveTouchController.StateTouch state)
    {
        switch (state)
        {
            case MoveTouchController.StateTouch.open:
            {
                AddedMoveModeSlow(3, 0.95f);
                break;
            }
            case MoveTouchController.StateTouch.none:
            {
                RemoveMoveModeSlow();
                break;
            }
        }
    }
}
