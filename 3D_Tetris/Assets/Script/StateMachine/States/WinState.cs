using Helper.Patterns.FSM;
using UnityEngine;

namespace Script.StateMachine.States
{
    public class WinState : AbstractState<TetrisState>
    {
        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            RealizationBox.Instance.gameManager.ShowWinPanel();
        }

        public override void Exit(TetrisState last)
        {
        }
    }
}