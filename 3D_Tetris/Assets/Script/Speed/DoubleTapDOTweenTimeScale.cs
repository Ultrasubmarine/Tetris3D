using System;
using DG.Tweening;
using UnityEngine;

namespace Script.Speed
{
    public class DoubleTapDOTweenTimeScale: MonoBehaviour
    {
        [SerializeField] private float _timeScale;
        private TetrisFSM _fsm;
        private void Start()
        {
            _fsm = RealizationBox.Instance.FSM;
            
            RealizationBox.Instance.tapsEvents.OnDoubleTap += OnDoubleTap;
            _fsm.AddListener(TetrisState.AllElementsDrop, ResetSpeed);
            _fsm.AddListener(TetrisState.OpenEvilBox, ResetSpeed);
        }

        public void OnDoubleTap()
        {
            var state = _fsm.GetCurrentState();
            if(state == TetrisState.CreateStar || state == TetrisState.OpenEvilBox)
                DOTween.timeScale = _timeScale;
        }

        public void ResetSpeed()
        {
            DOTween.timeScale = 1;
        }
    }
}