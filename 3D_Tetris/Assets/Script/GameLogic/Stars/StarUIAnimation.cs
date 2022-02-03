using System;
using DG.Tweening;
using UnityEngine;

namespace Script.GameLogic.Stars
{
    public class StarUIAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _oreol;
        [SerializeField] private float _starRotationSpeed;
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

        public Action OnAnimationEnd;
        public bool WaitingCollectAnimation { get; private set; }
        
        // Start is called before the first frame update
        void Start()
        {
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
                .Join(myTransform.DOAnchorPosY(_deltaMove + FinishPoint, _timeMoving).From(Vector2.up * FinishPoint).SetLoops(3, LoopType.Yoyo))
                .Append(_oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear/2))
                .Append(myTransform.DOAnchorPosY(-StarPanelTransform.sizeDelta.y/2, _timeDisappear))
                .Join(_starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear)).OnComplete(()=>
                {
                    _star.SetActive(true);
                    OnAnimationEnd?.Invoke();
                });
        }

        private void Update()
        {
            _rotation += Time.deltaTime *_starRotationSpeed;
            if (_rotation > 360.0f)
            {
                _rotation = 0.0f;
            }
            _oreol.localRotation = Quaternion.Euler(0, 0, _rotation);
        }

        public void StartAnimation()
        {
            _star.SetActive(true);

            animation.Rewind();
            animation.Play();
        }
    }
}