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
            
            if(!_starsManager.collectStarLvl)
                _FSM.SetNewState(TetrisState.GenerateElement);

            if (_starsManager.CanCreateStar())
            {
                _starsManager.OnCreatedStar += OnCreatedStar;
                _starsManager.CreateStar();
            }
            else
                _FSM.SetNewState(TetrisState.GenerateElement);
        }

        public override void Exit(TetrisState last)
        {
        }

        public void OnCreatedStar()
        {
            _starsManager.OnCreatedStar -= OnCreatedStar;
            _FSM.SetNewState(TetrisState.GenerateElement);
        }
        
    }
}