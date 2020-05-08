using System;
using UnityEngine;

namespace Script.Booster
{
    public enum BoosterState
    {
        ReadyForUse,
        Respawn,
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Booster", order = 1)]
    public class BoosterBase : ScriptableObject
    {
        public event Action<BoosterState> onStateChange;
        
        public BoosterState currentState;
        
        public Sprite icon => _icon;
        
        public  Timer timer => _timer;
        
        public float timeRespawn => _timeRespawn;
        
        
        [SerializeField] Sprite _icon;
        
        [SerializeField] private Timer _timer;
        
        [SerializeField] private float _timeRespawn;
        
        [SerializeField] private BoosterState _currentState;
        
        
        public BoosterBase()
        {
          
        }


        public void Initialize()
        {
            SetState(BoosterState.ReadyForUse); 
        }
        
        public virtual void Apply()
        {
            if (currentState == BoosterState.ReadyForUse)
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
                SetState(BoosterState.Respawn);
            }
        }

        public virtual void EndApply()
        {
            
        }

        protected void SetState(BoosterState newState)
        {
            currentState = newState;
            onStateChange?.Invoke(currentState);
        }
        
        protected virtual void OnRespawn()
        {
                
        }

        protected virtual void OnReadyForUse()
        {
            
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
            }
        }
    }
}