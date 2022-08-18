using Helper.Patterns.FSM;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class BigBombGenerationState  : AbstractState<TetrisState>
    {
        
        
        public override void Enter(TetrisState last)
        {
            RealizationBox.Instance.changeNewElementToBomb.ChangeToBigBomb(true);
            
            //  _heightHandler.CalculateHeight();
            base.Enter(last);
        }
        public override void Exit(TetrisState last)
        {
            
        }
    }
}