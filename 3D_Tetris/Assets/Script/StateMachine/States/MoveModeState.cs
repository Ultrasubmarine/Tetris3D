using System.Collections.Generic;
using Helper.Patterns.FSM;
using Script.Controller;

namespace Script.StateMachine.States
{
    public class MoveModeState : AbstractState<TetrisState>
    {
        private List<MovePointUi> _movePointUi;

        private MovePointsManager _movePointsManager;
        
        public MoveModeState()
        {
            _movePointsManager = RealizationBox.Instance.movePointsManager;
            _movePointUi = _movePointsManager.points;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            foreach (var point in _movePointUi)
            {
                point.onPointEnter += onPointTouch;
            }

            _movePointsManager.ShowPoints();
        }

        public override void Exit(TetrisState last)
        {
            foreach (var point in _movePointUi)
            {
                point.onPointEnter -= onPointTouch;
            }
        }

        private void onPointTouch(move direction)
        {
            _movePointsManager.ShowPoints();
        }

        private void onBreakTouch()
        {
            // exit in state
        }
    }
}