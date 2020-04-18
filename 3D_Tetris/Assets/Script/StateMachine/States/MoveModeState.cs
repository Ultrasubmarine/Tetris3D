using Helper.Patterns.FSM;
using Script.Controller;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class MoveModeState : AbstractState<TetrisState>
    {
        private MovePointsManager _movePointsManager;

        private GameController _gameController;

        private MoveTouchController _moveTouchController;

        private SlowManager _slowManager;

        
        public MoveModeState()
        {
            _movePointsManager = RealizationBox.Instance.movePointsManager;
            _gameController = RealizationBox.Instance.gameController;
            _moveTouchController = RealizationBox.Instance.moveTouchController;
            _slowManager = RealizationBox.Instance.slowManager;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

//            
//            if (Input.touchCount != 1)
//                OnBreakMode();
//            
//            _movePointsManager.onPointEnter += OnPointTouch;
////            _movePointsManager.ShowPoints();
//            _moveTouchController.onStateChanged += OnMoveTouchControllerStateChange;
//            
//            _slowManager.AddedMoveModeSlow(3, 0.5f);
        }

        public override void Exit(TetrisState last)
        {
//            _movePointsManager.onPointEnter -= OnPointTouch;
//            _moveTouchController.onStateChanged -= OnMoveTouchControllerStateChange;
//            _movePointsManager.HidePoints();
//            
//            if( last != TetrisState.Move)
//                _slowManager.RemoveMoveModeSlow();
        }

        private void OnPointTouch(move direction)
        {
            Debug.Log("MOVE DIRECTION");
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