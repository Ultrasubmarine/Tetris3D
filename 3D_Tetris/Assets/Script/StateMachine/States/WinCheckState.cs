using System.Diagnostics.Eventing.Reader;
using Helper.Patterns.FSM;
using Script.GameLogic.Stars;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class WinCheckState : AbstractState<TetrisState>
    {
        private Score _score;
        private GameCamera _gameCamera;
        private StarsManager _starsManager;

        public WinCheckState()
        {
            _score = RealizationBox.Instance.score;
            _gameCamera = RealizationBox.Instance.gameCamera;
            _starsManager = RealizationBox.Instance.starsManager;
        }

        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            
            if (_score.CheckWin() && _starsManager.CheckWin())
                _FSM.SetNewState(TetrisState.WinGame);
            else
            {
                if( _gameCamera.SetStabilization())
                    _gameCamera.onStabilizationEnd += OnStabilizationCameraEnd;
                else
                    _FSM.SetNewState(TetrisState.CreateStar);
            }
        }

        public void OnStabilizationCameraEnd()
        {
            _gameCamera.onStabilizationEnd -= OnStabilizationCameraEnd;
            _FSM.SetNewState(TetrisState.CreateStar);
        }
        
        public override void Exit(TetrisState last)
        {
        }
    }
}