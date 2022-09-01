using DG.Tweening;
using Helper.Patterns.FSM;
using Script.PlayerProfile;
using Script.Projections;

namespace Script.StateMachine.States
{
    public class LoseGameState : AbstractState<TetrisState>
    {
        private Projection _projection;
        private Ceiling _ceiling;
        private ProjectionLineManager _projLineManager;
        private PauseUI _pauseUI;
        
        public LoseGameState()
        {
            _ceiling = RealizationBox.Instance.ceiling;
            _projection = RealizationBox.Instance.projection;
            _projLineManager = RealizationBox.Instance.projectionLineManager;
            _pauseUI = RealizationBox.Instance.pauseUI;
        }

        public override void Enter(TetrisState last)
        {
            RealizationBox.Instance.bigBombGamePlayOffer.HideBtn();
            
            if (PlayerSaveProfile.instance._bestScore < RealizationBox.Instance.starsManager.collectedStars)
            {
                PlayerSaveProfile.instance.SetBestScore(RealizationBox.Instance.starsManager.collectedStars);
            }
            
            if (_pauseUI.isPause)
            {
                _pauseUI.onPauseStateChange += WaitPause;
            }
            else
            {
                Lose();
            }
            
            base.Enter(last);
        }

        private void Lose()
        {
            TrackManager.LvlFailed(RealizationBox.Instance.starsManager.collectedStars);
            
            _ceiling.Destroy();
            _projection.Destroy();
            _projLineManager.Clear();
            RealizationBox.Instance.gameManager.OnLoseGame();
            PlayerSaveProfile.instance.SetSkipMode(true);
        }
        
        public void WaitPause(bool pauseState)
        {
            if (!pauseState)
            {
                _pauseUI.onPauseStateChange -= WaitPause;
                _pauseUI.DOKill();
                Lose();
            }
        }
        
        public override void Exit(TetrisState last)
        {
        }
    }
}