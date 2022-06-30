using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Controller;
using Script.Controller.TouchController;
using UnityEngine;

public class SlowManager : MonoBehaviour
{
    [Header("Slow when turn")]
    [SerializeField] private float _time = 4;
    
    [SerializeField] private float _value = 0.95f;

    [SerializeField] private float _slowTimeInJoystickMode = 3f;
    
    [SerializeField] private float _slowProcInJoystickMode = 0.85f;

    [SerializeField] private float _slowFreezeElement = 0.85f;
    
    [SerializeField] private float _slowOffer = 0.7f;
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

    private bool _isPauseSlow;

    private bool _isFeezeSlow = false;

    private bool _isOfferSlow = false;
    
    private Slow? FreezeElementModeSlow;
    
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
    
    public void SetPauseSlow(bool isPause)
    {
        _isPauseSlow = isPause;
        CalculateSlow();
    }

    public void SetOfferSlow(bool isOfferSlow)
    {
        _isOfferSlow = isOfferSlow;
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

        /*timer.onStateChanged += (s) =>
        {
            if (s == TimerState.Completed)
                OnDestroyTurnModeTimer();
        };*/
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
        
        if (_isPauseSlow)
        {
            _slowlerValue = 1;
        }
        else if (_isFeezeSlow)
        {
            _slowlerValue = _slowFreezeElement;
        }
        else if (_isOfferSlow)
        {
            _slowlerValue = _slowOffer;
        }
        else
        {
            foreach (var slow in _slowsList)
            {
                _slowlerValue += slow.slow;
            }

            if (MoveModeSlow != null)
                _slowlerValue += MoveModeSlow.Value.slow;
        
            if (TurnModeSlow != null)
                _slowlerValue += TurnModeSlow.Value.slow;
        }
        
        onUpdateValue?.Invoke();
    }

    public void OnJoystickTouchChange(JoystickState state)
    {
        switch (state)
        {
            case JoystickState.Show:
            {
                AddedMoveModeSlow(_slowTimeInJoystickMode, _slowProcInJoystickMode);
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

    public void FreezeElementSlowOn()
    {
        _isFeezeSlow = true;
        RealizationBox.Instance.FSM.onStateChange += FreezeElementSlowOff;
   //     DOTween.To(() => _slowlerValue, x => _slowlerValue = x, _slowFreezeElement, 0.1f).OnUpdate(()=>onUpdateValue?.Invoke());
        CalculateSlow(); 
    }

    public void FreezeElementSlowOff( TetrisState state)
    {
        if (state == TetrisState.MergeElement || state == TetrisState.GenerateElement)
        {
            _isFeezeSlow = false;
            RealizationBox.Instance.FSM.onStateChange += FreezeElementSlowOff;
            CalculateSlow();  
        }
    }
    
}
