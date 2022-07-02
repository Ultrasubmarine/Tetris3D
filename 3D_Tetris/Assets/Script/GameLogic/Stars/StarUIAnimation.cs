using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Script.GameLogic.Stars
{
    public enum StarPause
    {
         none,
         show,
         dissapear,
    }
    
    public class StarUIAnimation : MonoBehaviour
    {
        [FormerlySerializedAs("Stars")] [SerializeField] private MiniStar[] _miniStars;
        
        private bool _dissapearAfterComplete = false;
        
        public Action OnAnimationEnd;
        public Action OnUpdateStartScoreText;
        
       
        public bool WaitingCollectAnimation { get; private set; }

        private int _collectStarsInAnimation;
        
        //oreol
        private Transform _oreolTransform;
        [SerializeField] private float _starRotationSpeed = 10;
        private float _rotation;

        [SerializeField] private CanvasGroup _star2;
        [SerializeField] private CanvasGroup _oreol2;
        
        //animation
        private Sequence animation;
        private RectTransform myTransform;
        [SerializeField] private RectTransform StarPanelTransform;
        
        [SerializeField] private float _time;
        [SerializeField] private float _deltaMove = 20f;
        
        [SerializeField] private float _timeAlpha = 1.5f / 2;
        [SerializeField] private float _timeAlphaOreol = 1.5f;
        [SerializeField] private float _timeMoving;
        [SerializeField] private float _timeDisappear = 1;

        //for upd score text
        private bool _isScoreTextUpdated = false;
        private float _currentTime = 0;

        private StarPause _pauseOn;
        
        [SerializeField] private MiniStarUIAnimation _miniStarUIAnimation;
        // Start is called before the first frame update
        void Start()
        {
            _collectStarsInAnimation = 0;
            myTransform = GetComponent<RectTransform>();
            _oreolTransform = _oreol2.GetComponent<Transform>();

            float FinishPoint = -StarPanelTransform.sizeDelta.y + StarPanelTransform.anchoredPosition.y;
            
            animation = DOTween.Sequence().SetAutoKill(false).Pause();
            animation.Append(myTransform.DOAnchorPosY(FinishPoint, _time / 2).From(Vector2.down * Screen.height / 2))
                .Join(_star2.DOFade(1f, _time / 2.5f).From(0f))
                .Append(_oreol2.DOFade(1f, _timeAlphaOreol).From(0f))
                .Join(myTransform.DOAnchorPosY(_deltaMove + FinishPoint, _timeMoving)
                    .From(Vector2.up * FinishPoint).SetLoops(3, LoopType.Yoyo)).OnComplete(() => OnShowEnd())
                //HIDE PART
                .Append(_oreol2.DOFade(0f, _timeDisappear / 2).From(1f))
                .Join(_miniStars[0].oreolRender.DOFade(0f, _timeDisappear / 2).From(1f))
                .Join(_miniStars[1].oreolRender.DOFade(0f, _timeDisappear / 2).From(1f))
                .Join(_miniStars[2].oreolRender.DOFade(0f, _timeDisappear / 2).From(1f))
                .Join(_miniStars[3].oreolRender.DOFade(0f, _timeDisappear / 2).From(1f))
                .Join(_miniStars[4].oreolRender.DOFade(0f, _timeDisappear / 2).From(1f))
                .Append(myTransform.DOAnchorPosY(-StarPanelTransform.sizeDelta.y / 4, _timeDisappear).OnUpdate(() =>
                {
                    _currentTime += Time.deltaTime;
                    if (!_isScoreTextUpdated && _currentTime > _timeDisappear * 0.75f)
                    {
                        OnUpdateStartScoreText?.Invoke();
                        _isScoreTextUpdated = true;
                    }
                }))
                .Join(_star2.DOFade(0f, _timeDisappear).From(1f))
                .Join(_miniStars[0].starCanvasGroup.DOFade(0f, _timeDisappear).From(1f))
                .Join(_miniStars[1].starCanvasGroup.DOFade(0f, _timeDisappear).From(1f))
                .Join(_miniStars[2].starCanvasGroup.DOFade(0f, _timeDisappear).From(1f))
                .Join(_miniStars[3].starCanvasGroup.DOFade(0f, _timeDisappear).From(1f))
                .Join(_miniStars[4].starCanvasGroup.DOFade(0f, _timeDisappear).From(1f));
            
            animation.OnComplete(() =>
            {
                RealizationBox.Instance.FSM.onStateChange -= onFSMStateChange;
                _isScoreTextUpdated = false;
                _currentTime = 0;
                _collectStarsInAnimation = 0;
                _dissapearAfterComplete = false;
                
                _miniStars[0].gameObject.SetActive(false);
                _miniStars[1].gameObject.SetActive(false);
                _miniStars[2].gameObject.SetActive(false);
                _miniStars[3].gameObject.SetActive(false);
                _miniStars[4].gameObject.SetActive(false);
                
                OnAnimationEnd?.Invoke();
            });

           animation.OnPlay(() => RealizationBox.Instance.FSM.onStateChange += onFSMStateChange);
            // animationDissapear.OnPlay(() =>
            //  {
            //  _miniStarUIAnimation.DissapearStars(_collectStarsInAnimation-1);
            //  });
            animation.Complete();
        }

        public void OnShowEnd()
        {
            animation.Pause();
            
            if (_dissapearAfterComplete
                || RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.WinCheck 
                || RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.WinGame)
            {
                animation.Play();
            }
        }
        private void Update()
        {
            if (_isScoreTextUpdated) return;
            
            _rotation += Time.deltaTime *_starRotationSpeed;
            if (_rotation > 360.0f)
            {
                _rotation = 0.0f;
            }
            _oreolTransform.localRotation = Quaternion.Euler(0, 0, _rotation);
        }

        public void StartAnimation()
        {
            _collectStarsInAnimation++;
            _isScoreTextUpdated = false;
            
            if(_collectStarsInAnimation < 2)
            { 
               // _star.SetActive(true);
                animation.Rewind();
                animation.Play();
            }
            else
            {
                if (_collectStarsInAnimation < 3)
                {
            //        _miniStarUIAnimation.onAnimationFinished += OnMiniStarsFinished;
                }
                _miniStarUIAnimation.ShowMiniStar(_collectStarsInAnimation-2);
            }
        }

        public void OnMiniStarsFinished(int index)
        {
            if (index == _collectStarsInAnimation - 2)
            {
                _miniStarUIAnimation.onAnimationFinished -= OnMiniStarsFinished;
            }
                
        }

        public void onFSMStateChange( TetrisState state)
        {
            if ( (state == TetrisState.CreateStar || state == TetrisState.LoseGame || state == TetrisState.WinGame)  && _collectStarsInAnimation > 0)
            {
                if (_collectStarsInAnimation == 1 && animation.IsPlaying()) //wait animation
                {
                    _dissapearAfterComplete = true;
                }
                else
                    animation.Play(); //DissapearAnimation();
            }
            if ((state == TetrisState.OpenEvilBox) && RealizationBox.Instance.evilBoxManager.CanOpenBox())
            {
                if (animation.IsPlaying()) //wait animation
                {
                    _dissapearAfterComplete = true;
                }
                else
                    animation.Play(); //DissapearAnimation();
            }
        }

        public void Clear()
        {
            animation.Rewind();
            animation.Complete();
            
            if (_collectStarsInAnimation > 1)
            {
                _miniStarUIAnimation.onAnimationFinished -= OnMiniStarsFinished;
                _miniStarUIAnimation.Clear(_collectStarsInAnimation - 1);
                
            }
            _collectStarsInAnimation = 1;
            
            _isScoreTextUpdated = false;
            _currentTime = 0;
            _collectStarsInAnimation = 0;
            _dissapearAfterComplete = false;
        }

        public void Pause(bool isPause)
        {
            
          //  _miniStarUIAnimation.Pause(isPause);
            if (isPause)
            {
                if (animation.IsPlaying())
                {
                    _pauseOn = StarPause.show;
                    animation.Pause();
                }
                else
                {
                    _pauseOn = StarPause.none;
                }
            }
            else
            {
                switch (_pauseOn)
                {
                    case StarPause.none: return;
                    case StarPause.show:
                    {
                        animation.Play();
                        break;
                    }
                }
            }
        }
    }
}