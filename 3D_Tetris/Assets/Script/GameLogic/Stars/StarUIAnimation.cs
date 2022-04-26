using System;
using DG.Tweening;
using UnityEngine;

namespace Script.GameLogic.Stars
{
    public class StarUIAnimation : MonoBehaviour
    {
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
                if ((_collectStarsInAnimation == 1 && _dissapearAfterComplete ) || RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.WinGame)
                {
                    DissapearAnimation();
                }
            });
            
            animationDissapear = DOTween.Sequence().SetAutoKill(false).Pause();
            animationDissapear.Append(_oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2))
                .Append(myTransform.DOAnchorPosY(-StarPanelTransform.sizeDelta.y / 4, _timeDisappear).OnUpdate(() =>
                {
                    timerDissapear += Time.deltaTime;
                    if (isStarUIShow && timerDissapear > _timeDisappear * 0.75f)
                    {
                        OnUpdateStartScoreText?.Invoke();
                        isStarUIShow = false;
                    }
                }))
                .Join(_starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear));

            animationDissapear.OnComplete(() =>
            {
                isStarUIShow = false;
                timerDissapear = 0;
                _collectStarsInAnimation = 0;
                _dissapearAfterComplete = false;
                _startDissapear = false;
                OnAnimationEnd?.Invoke();
            });

            animationDissapear.OnPlay(() =>
            {
                _miniStarUIAnimation.DissapearStars(_collectStarsInAnimation-1);
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
            
            animation.Complete();
            animationDissapear.Rewind();
            _startDissapear = true;
            animationDissapear.Play();
        }

        public void onFSMStateChange( TetrisState state)
        {
            if (state == TetrisState.CreateStar && _collectStarsInAnimation > 0)
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
    }
}