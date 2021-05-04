using Helper.Patterns.FSM;
using Script.Projections;

namespace Script.StateMachine.States
{
    public class LoseGameState : AbstractState<TetrisState>
    {
        private Projection _projection;
        private Ceiling _ceiling;
        private ProjectionLineManager _projLineManager;
        
        public LoseGameState()
        {
            _ceiling = RealizationBox.Instance.ceiling;
            _projection = RealizationBox.Instance.projection;
            _projLineManager = RealizationBox.Instance.projectionLineManager;
        }

        public override void Enter(TetrisState last)
        {
            _ceiling.Destroy();
            _projection.Destroy();
            _projLineManager.Clear();
            base.Enter(last);
        }

        public override void Exit(TetrisState last)
        {
        }
    }
}