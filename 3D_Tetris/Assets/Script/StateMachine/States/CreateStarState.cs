using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Helper.Patterns.FSM;
using Script.GameLogic.Stars;
using Script.GameLogic.TetrisElement;

namespace Script.StateMachine.States
{
    public class CreateStarState : AbstractState<TetrisState>
    {
        private PlaneMatrix _matrix;
        private StarsManager _starsManager;

        public CreateStarState ()
        {
            _myState = TetrisState.CreateStar;

            _matrix = RealizationBox.Instance.matrix;
            _starsManager = RealizationBox.Instance.starsManager;
        }

        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            if (!_starsManager.collectStarLvlLvl)
            {
                _FSM.SetNewState(TetrisState.GenerateElement);
                return;
            }
                

            if (_starsManager.onCollectedAnimationWaiting) // if u collect u dont create star in this step
            {
                _starsManager.OnCollectedStars += OnCollectedStar;
            }
            else if (_starsManager.CanCreateStar()) 
            {
                 _starsManager.OnCreatedStar += OnCreatedStar;
                 _starsManager.CreateStar();
            }
            else
                _FSM.SetNewState(TetrisState.GenerateElement);
        }

        public override void Exit(TetrisState last)
        {
            _starsManager.OnCollectedStars -= OnCollectedStar;
            _starsManager.OnCreatedStar -= OnCreatedStar;
        }

        public void OnCreatedStar()
        {
            _starsManager.OnCreatedStar -= OnCreatedStar;
            if (_FSM.GetCurrentState() == TetrisState.Restart)
                return;
            _FSM.SetNewState(TetrisState.GenerateElement);
        }

        public void OnCollectedStar()
        {
            _starsManager.OnCollectedStars -= OnCollectedStar;
            if (_FSM.GetCurrentState() == TetrisState.Restart)
                return;
            _FSM.SetNewState(TetrisState.GenerateElement);
        }
    }
}