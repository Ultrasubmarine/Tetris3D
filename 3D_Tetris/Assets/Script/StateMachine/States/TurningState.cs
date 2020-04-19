using DG.Tweening;
using Helper.Patterns.FSM;
using Script.Influence;
using UnityEngine;

namespace Script.StateMachine.States
{
    
    public class TurningState : AbstractState<TetrisState>
    {
        private InfluenceManager _influence;

        private Transform _place;

        private float _timeTurn = 0.5f;

        private int _rotate = 0;
        
        public TurningState()
        {
            _influence = RealizationBox.Instance.influenceManager;
            _place = RealizationBox.Instance.place;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            _influence.enabled = false;

            _rotate = (_rotate + 90) % 360;
            _place.DORotate(new Vector3(0, _rotate, 0), _timeTurn).OnComplete( () => _FSM.SetNewState(last));
        }
        public override void Exit(TetrisState last)
        {
            _influence.enabled = true;
        }
    }
}