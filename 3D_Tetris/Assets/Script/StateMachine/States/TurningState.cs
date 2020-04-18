using Helper.Patterns.FSM;
using Script.Influence;
using UnityEngine;

namespace Script.StateMachine.States
{
    
    public class TurningState : AbstractState<TetrisState>
    {
        private InfluenceManager _influence;

        private Transform _place;
        
        
        public TurningState()
        {
            _influence = RealizationBox.Instance.influenceManager;
        }
        
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            _influence.enabled = false;
            
            
            
            if (last != TetrisState.WaitInfluence && last != TetrisState.EndInfluence && last != TetrisState.GenerateElement)
            {
                InfluenceData.delayedDrop = true;
                return;
            }
        
            var empty = _matrix.CheckEmptyPlaсe(ElementData.NewElement, new Vector3Int(0, -1, 0));
            if (empty)
            {
                _elementDropper.StartDropElement();
            
            
                _FSM.SetNewState(TetrisState.WaitInfluence);
                return;
            }

            _FSM.SetNewState(TetrisState.MergeElement);
        }
        public override void Exit(TetrisState last)
        {
            _influence.enabled = true;
            throw new System.NotImplementedException();
        }
    }
}