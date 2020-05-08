using System;
using UnityEngine;

namespace Script.Booster
{
    public enum BoosterState
    {
        ReadyForUse,
        UseWithCountdown,
        Respawn,
    }
    
    [CreateAssetMenu(fileName = "BaseBooser", menuName = "ScriptableObjects/Booster/BaseBooster", order = 1)]
    public class BoosterBase : ScriptableObject
    {
        public event Action<BoosterState> onStateChange;
        
        public BoosterState currentState => _currentState;
        
        public Sprite icon => _icon;
        
        public  Timer timer => _timer;
        public  Timer useTimer => _useTimer;

        
        [SerializeField] Sprite _icon;

        [SerializeField] private float _timeRespawn;

        protected float _useTime;
        
        private Timer _timer;
        
        private Timer _useTimer;
        
        protected BoosterState _currentState;
        
        
        public virtual void Initialize()
        {
            onStateChange += OnStateChange;
            SetState(BoosterState.ReadyForUse); 
        }
        
        public virtual void Apply()
        {
            if (_currentState == BoosterState.ReadyForUse)
                SetState(BoosterState.Respawn);
        }

        public virtual void EndApply()
        {
            
        }

        protected void SetState(BoosterState newState)
        {
            _currentState = newState;
            onStateChange?.Invoke(_currentState);
        }
        
        protected virtual void OnRespawn()
        {
            _timer = TimersKeeper.Schedule(_timeRespawn);
            _timer.onStateChanged += (s) =>
            {
                if(s == TimerState.Completed)
                {
                    SetState(BoosterState.ReadyForUse);
                    _timer = null;
                }
            }; 
        }

        protected virtual void OnReadyForUse()
        {
            
        }

        protected virtual void UseWithCountdown()
        {
            _useTimer = TimersKeeper.Schedule(_useTime);
            _useTimer.onStateChanged += (s) =>
            {
                if(s == TimerState.Completed)
                {
                    SetState(BoosterState.Respawn);
                    _timer = null;
                }
            }; 
        }
        
        protected void OnStateChange(BoosterState newState)
        {
            switch (newState)
            {
                case BoosterState.ReadyForUse:
                {
                    OnReadyForUse();
                    break;
                }
                case BoosterState.Respawn:
                {
                    OnRespawn();
                    break;
                }
                case BoosterState.UseWithCountdown:
                {
                    UseWithCountdown();
                    break;
                }
            }
        }
    }
}