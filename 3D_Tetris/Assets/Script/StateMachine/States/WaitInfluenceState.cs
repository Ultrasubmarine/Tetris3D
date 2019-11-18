using Helper.Patterns.FSM;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class WaitInfluenceState : AbstractState<TetrisState>
    {
        private GameObject _gameObject;

        public WaitInfluenceState()
        {
            _gameObject = RealizationBox.Instance.controller;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            _gameObject.SetActive(true);
        }

        public override void Exit(TetrisState last)
        {
            _gameObject.SetActive(false);
        }
    }
}