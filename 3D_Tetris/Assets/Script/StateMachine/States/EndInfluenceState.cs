using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class EndInfluenceState : AbstractState<TetrisState>
    {
        public EndInfluenceState()
        {
            
        }
        public override void Enter(TetrisState last)
        {
            if (InfluenceData.delayedDrop)
            {
                InfluenceData.delayedDrop = false;
                Debug.Log("EndInfluense check");
                _FSM.SetNewState(TetrisState.Drop);
            }
            else
                _FSM.SetNewState(TetrisState.WaitInfluence);
        }

        public override void Exit(TetrisState last)
        {
        }
    }
}