using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;

namespace Script.StateMachine.States
{
    public class EvilBoxAddBlockState : AbstractState<TetrisState>
    {
        private PlaneMatrix _matrix;
        private HeightHandler _heightHandler;
        private GameLogicPool _gameLogicPool;

        public EvilBoxAddBlockState()
        {
            _matrix = RealizationBox.Instance.matrix;
            _heightHandler = RealizationBox.Instance.haightHandler;
            _gameLogicPool = RealizationBox.Instance.gameLogicPool;
            
        }
        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            
            RealizationBox.Instance.evilBoxManager.OpenEvilBox();
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        }

        public override void Exit(TetrisState last)
        {
            
        }
    }
}