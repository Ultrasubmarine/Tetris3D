using Helper.Patterns.FSM;
using Script.GameLogic;
using Script.GameLogic.Stars;
using Script.GameLogic.TetrisElement;

namespace Script.StateMachine.States
{
    public class EvilBoxAddBlockState : AbstractState<TetrisState>
    {
        private PlaneMatrix _matrix;
        private HeightHandler _heightHandler;
        private GameLogicPool _gameLogicPool;
        private StarsManager _starsManager;
        private EvilBoxManager _evilBoxManager;

        public EvilBoxAddBlockState()
        {
            _matrix = RealizationBox.Instance.matrix;
            _heightHandler = RealizationBox.Instance.haightHandler;
            _gameLogicPool = RealizationBox.Instance.gameLogicPool;
            _starsManager = RealizationBox.Instance.starsManager;
            _evilBoxManager = RealizationBox.Instance.evilBoxManager;
        }
        public override void Enter(TetrisState last)
        {
            base.Enter(last);

            if (!_evilBoxManager.CanOpenBox())
            {
                _FSM.SetNewState(TetrisState.AllElementsDrop);
                return;
            }
            
            if ( _starsManager.onCollectedAnimationWaiting) // if u collect u dont create star in this step
            {
                _starsManager.OnCollectedStars += OnCollectedStar;
                return;
            }

            _evilBoxManager.OpenEvilBox();
            _evilBoxManager.OnOpenBoxEnded += OnOpenBoxEnded;
        }

        public void OnCollectedStar()
        {
            _starsManager.OnCollectedStars -= OnCollectedStar;
            
            _evilBoxManager.OpenEvilBox();
            _evilBoxManager.OnOpenBoxEnded += OnOpenBoxEnded;
        }
        
        
        public void OnOpenBoxEnded()
        {
            _evilBoxManager.OnOpenBoxEnded -= OnOpenBoxEnded;
            
            RealizationBox.Instance.elementDropper.SetAllDropFastSpeed(0.7f);
            _FSM.SetNewState(TetrisState.AllElementsDrop);
        }

        public override void Exit(TetrisState last)
        {
            
        }
    }
}