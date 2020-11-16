using System;
using System.Collections.Generic;
using Script.Controller;
using Script.Controller.TouchController;
using UnityEngine;

public class SlowManager : MonoBehaviour
{
    [Header("Slow when turn")]
    [SerializeField] private float _time = 4;
    
    [SerializeField] private float _value = 0.95f;

    
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

    private IslandTurn _islandTurn;
    
    public float slow => _slowlerValue;

    public Action onUpdateValue;
    
    
    private List<Slow> _slowsList = new List<Slow>();

    private float _slowlerValue = 0;

    private Slow? MoveModeSlow;
    
    private Slow? TurnModeSlow;
    
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
    
    private void AddedTurnModeSlow(float time, float value)
    {
        var timer = TimersKeeper.Schedule(time);
        var slow = new Slow(timer, value);

        timer.onStateChanged += (s) =>
        {
            if (s == TimerState.Completed)
                OnDestroyTurnModeTimer();
        };
        TurnModeSlow = slow;
        
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
    
    private void RemoveTurnModeSlow()
    {
        if (TurnModeSlow == null)
            return;
        
        TurnModeSlow.Value.timer.Cancel();
        TurnModeSlow = null;
        CalculateSlow();
    }

    private void Start()
    {
        _moveTouchController = RealizationBox.Instance.moveTouchController;
        _islandTurn = RealizationBox.Instance.islandTurn;

        _islandTurn.OnStartTurn += OnTurnIsland;
        _islandTurn.OnEndTurn += OnFinishTurnIsland;
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
    
    private void OnDestroyTurnModeTimer()
    {
        TurnModeSlow = null;
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
        
        if (TurnModeSlow != null)
            _slowlerValue += TurnModeSlow.Value.slow;
        
        onUpdateValue?.Invoke();
    }

    public void OnJoystickTouchChange(JoystickState state)
    {
        switch (state)
        {
            case JoystickState.Show:
            {
                AddedMoveModeSlow(3, 0.95f);
                break;
            }
            case JoystickState.Hide:
            {
                RemoveMoveModeSlow();
                break;
            }
        }
    }

    public void DeleteAllSlows()
    {
        foreach (var slow1 in _slowsList)
        {
            slow1.timer.Cancel();
        }
        _slowsList.Clear();
        OnDestroyMoveModeTimer();
        OnDestroyTurnModeTimer();
        CalculateSlow();
    }
    
    private void OnTurnIsland()
    {
        AddedTurnModeSlow(_time, _value);
    }

    private void OnFinishTurnIsland()
    {
        RemoveTurnModeSlow();
    }
}
