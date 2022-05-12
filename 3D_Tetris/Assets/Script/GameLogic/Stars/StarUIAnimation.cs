using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private Transform _oreol;
        [SerializeField] private float _starRotationSpeed = 10;
        private float _rotation;

        [SerializeField] private GameObject _star;
        [SerializeField] private MeshRenderer _starMesh;
        [SerializeField] private SpriteRenderer _oreolRender;
        
        private RectTransform myTransform;
    
        [SerializeField] private float _time;
        [SerializeField] private float _yMove;
        [SerializeField] private float _deltaMove = 20f;
        
        [SerializeField] private float _timeAlphaStar = 1.5f;
        [SerializeField] private float _timeDelayBetweenAlphaStar = 1.5f;
        [SerializeField] private float _timeAlphaOreol = 1.5f;
        [SerializeField] private float _timeMoving;
        [SerializeField] private float _timeDisappear = 1;
        [SerializeField] private RectTransform StarPanelTransform;
        
        private Sequence animation;
        private Sequence animationDissapear;
        
        private bool isStarUIShow = false;
        private float timerDissapear = 0;

        private bool _startDissapear = false;

        private StarPause _pauseOn;
        
        [SerializeField] private MiniStarUIAnimation _miniStarUIAnimation;
        // Start is called before the first frame update
        void Start()
        {
            _collectStarsInAnimation = 0;
            myTransform = GetComponent<RectTransform>();
            
            var m =  _starMesh.material;
            var m2 =  _oreolRender.color;

            float FinishPoint = -StarPanelTransform.sizeDelta.y + StarPanelTransform.anchoredPosition.y;
            
            animation = DOTween.Sequence().SetAutoKill(false).Pause();
            animation.Append(_starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 1f), _timeAlphaStar)
                    .From(new Color(m.color.r, m.color.g, m.color.b, 0f))) //.SetLoops(3, LoopType.Yoyo))
                .Join(myTransform.DOAnchorPosY(FinishPoint, _time / 2).From(Vector2.down * Screen.height / 2))
                .Append(_oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 1f), _timeAlphaOreol)
                    .From(new Color(m2.r, m2.g, m2.b, 0f)))
                .Join(myTransform.DOAnchorPosY(_deltaMove + FinishPoint, _timeMoving).From(Vector2.up * FinishPoint)
                    .SetLoops(3, LoopType.Yoyo));

            animation.OnComplete(() =>
            {
                if ((_collectStarsInAnimation == 1 && _dissapearAfterComplete ) 
                    || RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.WinCheck 
                    || RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.WinGame)
                {
                    DissapearAnimation();
                }
            });
            
            animationDissapear = DOTween.Sequence().SetAutoKill(false).Pause();
            animationDissapear.Append(_oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2))
                .Join(_miniStars[0].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 1f)))
                .Join(_miniStars[1].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 1f)))
                .Join(_miniStars[2].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 1f)))
                .Join(_miniStars[3].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 1f)))
                .Join(_miniStars[4].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 1f)))
                .Append(myTransform.DOAnchorPosY(-StarPanelTransform.sizeDelta.y / 4, _timeDisappear).OnUpdate(() =>
                {
                    timerDissapear += Time.deltaTime;
                    if (isStarUIShow && timerDissapear > _timeDisappear * 0.75f)
                    {
                        OnUpdateStartScoreText?.Invoke();
                        isStarUIShow = false;
                    }
                }))
                .Join(_starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear))
                .Join(_miniStars[0].starMesh.material.
                    DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear).From(new Color(m.color.r, m.color.g, m.color.b, 1f)))
                .Join(_miniStars[1].starMesh.material
                    .DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear).From(new Color(m.color.r, m.color.g, m.color.b, 1f)))
                .Join(_miniStars[2].starMesh.material
                    .DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear).From(new Color(m.color.r, m.color.g, m.color.b, 1f)))
                .Join(_miniStars[3].starMesh.material
                    .DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear).From(new Color(m.color.r, m.color.g, m.color.b, 1f)))
                .Join(_miniStars[4].starMesh.material
                    .DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear).From(new Color(m.color.r, m.color.g, m.color.b, 1f)));

            animationDissapear.OnComplete(() =>
            {
                isStarUIShow = false;
                timerDissapear = 0;
                _collectStarsInAnimation = 0;
                _dissapearAfterComplete = false;
                _startDissapear = false;
                OnAnimationEnd?.Invoke();
                
                _miniStars[0].gameObject.SetActive(false);
                _miniStars[1].gameObject.SetActive(false);
                _miniStars[2].gameObject.SetActive(false);
                _miniStars[3].gameObject.SetActive(false);
                _miniStars[4].gameObject.SetActive(false);
            });

            animationDissapear.OnPlay(() =>
            {
              //  _miniStarUIAnimation.DissapearStars(_collectStarsInAnimation-1);
            });

            RealizationBox.Instance.FSM.onStateChange += onFSMStateChange;
        }

        private void Update()
        {
            if (!isStarUIShow) return;
            
            _rotation += Time.deltaTime *_starRotationSpeed;
            if (_rotation > 360.0f)
            {
                _rotation = 0.0f;
            }
            _oreol.localRotation = Quaternion.Euler(0, 0, _rotation);
        }

        public void StartAnimation()
        {
            _collectStarsInAnimation++;
            isStarUIShow = true;
            
            if(_collectStarsInAnimation < 2)
            { 
                _star.SetActive(true);
                animation.Rewind();
                animation.Play();
            }
            else
            {
                if (_collectStarsInAnimation < 3)
                {
                    _miniStarUIAnimation.onAnimationFinished += OnMiniStarsFinished;
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
        public void DissapearAnimation()
        {
            if (_startDissapear)
                return;
            
            _miniStarUIAnimation.DissapearStars(_collectStarsInAnimation-1);
            animation.Complete();
            animationDissapear.Rewind();
            _startDissapear = true;
            animationDissapear.Play();
            

            if (RealizationBox.Instance.pauseUI.isPause)
                Pause(true);
        }

        public void onFSMStateChange( TetrisState state)
        {
            if ( (state == TetrisState.CreateStar || state == TetrisState.LoseGame || state == TetrisState.WinGame)  && _collectStarsInAnimation > 0)
            {
                if (_collectStarsInAnimation == 1 && !animation.IsComplete()) //wait animation
                {
                    _dissapearAfterComplete = true;
                }
                else
                    DissapearAnimation();
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
            animationDissapear.Rewind();
            animationDissapear.Complete();

           
            isStarUIShow = false;
            timerDissapear = 0;
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
                else if (animationDissapear.IsPlaying())
                {
                    _pauseOn = StarPause.dissapear;
                    animationDissapear.Pause();
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
                    case StarPause.dissapear:
                    {
                        animationDissapear.Play();
                        break;
                    }
                }
            }
            
        }
    }
}