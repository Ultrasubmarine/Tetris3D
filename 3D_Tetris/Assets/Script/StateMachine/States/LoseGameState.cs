using Helper.Patterns.FSM;

namespace Script.StateMachine.States
{
    public class LoseGameState : AbstractState<TetrisState>
    {
        public LoseGameState()
        {
        }

        public override void Enter(TetrisState last)
        {
            base.Enter(last);
        }

        public override void Exit(TetrisState last)
        {
        }
    }
}