using System.Collections.Generic;
using Helper.Patterns.FSM;
using Script.Controller;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class MoveModeState : AbstractState<TetrisState>
    {
        private List<MovePointUi> _movePointUi;

        private MovePointsManager _movePointsManager;

        private GameController _gameController;

        private MoveTouchController _moveTouchController;
        
        public MoveModeState()
        {
            _movePointsManager = RealizationBox.Instance.movePointsManager;
            _gameController = RealizationBox.Instance.gameController;
            _moveTouchController = RealizationBox.Instance.moveTouchController;
            
            _movePointUi = _movePointsManager.points;
            
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            foreach (var point in _movePointUi)
            {
                point.onPointEnter += OnPointTouch;
            }

            _movePointsManager.ShowPoints();
            _moveTouchController.onStateChanged += OnMoveTouchControllerStateChange;
            
        }

        public override void Exit(TetrisState last)
        {
            foreach (var point in _movePointUi)
            {
                point.onPointEnter -= OnPointTouch;
            }
            _moveTouchController.onStateChanged -= OnMoveTouchControllerStateChange;
            _movePointsManager.HidePoints();
        }

        private void OnPointTouch(move direction)
        {
            _gameController.Move(direction);
        }

        private void OnMoveTouchControllerStateChange( MoveTouchController.StateTouch state)
        {
            if (state == MoveTouchController.StateTouch.none)
                OnBreakMode();
        }
        
        private void OnBreakMode()
        {
            _FSM.SetNewState(TetrisState.WaitInfluence);
        }
    }
}