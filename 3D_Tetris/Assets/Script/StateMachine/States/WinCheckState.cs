using System.Diagnostics.Eventing.Reader;
using Helper.Patterns.FSM;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class WinCheckState : AbstractState<TetrisState>
    {
        private Score _score;
        private GameCamera _gameCamera;

        public WinCheckState()
        {
            _score = RealizationBox.Instance.score;
            _gameCamera = RealizationBox.Instance.gameCamera;
        }

        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            if (_score.CheckWin())
                _FSM.SetNewState(TetrisState.WinGame);
            else
            {
                if( _gameCamera.SetStabilization())
                    _gameCamera.onStabilizationEnd += OnStabilizationCameraEnd;
                else
                    _FSM.SetNewState(TetrisState.GenerateElement);
            }
        }

        public void OnStabilizationCameraEnd()
        {
            _gameCamera.onStabilizationEnd -= OnStabilizationCameraEnd;
            _FSM.SetNewState(TetrisState.GenerateElement);
        }
        
        public override void Exit(TetrisState last)
        {
        }
    }
}