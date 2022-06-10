using Helper.Patterns.FSM;
using Script.GameLogic;
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

            if (RealizationBox.Instance.evilBoxManager.OpenEvilBox())
                RealizationBox.Instance.evilBoxManager.OnOpenBoxEnded += OnOpenBoxEnded;
            else
                _FSM.SetNewState(TetrisState.AllElementsDrop);
        }

        public void OnOpenBoxEnded()
        {
            RealizationBox.Instance.evilBoxManager.OnOpenBoxEnded -= OnOpenBoxEnded;
            
            RealizationBox.Instance.elementDropper.SetAllDropFastSpeed(0.7f);
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        }

        public override void Exit(TetrisState last)
        {
            
        }
    }
}