using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Script.Offers
{
    public class NextBigBombOffer: MonoBehaviour
    {
        private PlaneMatrix _matrix;
        private HeightHandler _height;
        
        [SerializeField] private RectTransform _bombIcon;
        [SerializeField] private CanvasGroup _offer;

        [SerializeField] private int _betweenOffersSteps = 3;
        [SerializeField] private int _inOneGameMax = 2;
        private int _betweenOffersStepsCurrent;
        private int _inOneGameCurrent;
        
        [SerializeField] private int needOutOfLimitAmount = 5;
        [SerializeField] private int maxLessOfLimit = 1;
        
        private bool _isShow = false;
        
        //Animation
        private Sequence _show, _hide;
        [SerializeField] private float _yMove = -100;
        [SerializeField] private float _time = 0.5f;
        
        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _height = RealizationBox.Instance.haightHandler;
            
            RealizationBox.Instance.FSM.AddListener(TetrisState.GenerateElement, CheckShowOffer);
            RealizationBox.Instance.FSM.AddListener(TetrisState.LoseGame, Hide);
            RealizationBox.Instance.FSM.AddListener(TetrisState.WinGame, Hide);

            var rectTransform = _offer.GetComponent<RectTransform>();
            
            _show = DOTween.Sequence().SetAutoKill(false).Pause();
            _show.Append(_offer.DOFade(1, _time / 2).From(0))
                .Join(rectTransform.DOAnchorPosY(0, _time / 2).From(Vector2.up * _yMove));

            _hide = DOTween.Sequence().SetAutoKill(false).Pause();
            _hide.Append(_offer.DOFade(0, _time / 2).From(1))
                .Join(rectTransform.DOAnchorPosY(_yMove, _time / 2).From(Vector2.zero)).OnComplete( () =>
                {
                    _bombIcon.DOKill();
                    _offer.gameObject.SetActive(false);
                    _hide.Rewind();
                });
            
            Clear();
           // _show.Play();
        }

        public void CheckShowOffer()
        {
            if (_inOneGameCurrent > _inOneGameMax)
                return;

            if (_betweenOffersStepsCurrent < _betweenOffersSteps)
            {
                _betweenOffersStepsCurrent++;
                return;
            }
            
            int yLimit = _height.limitHeight - 3;
            int outOfLimitAmount = 0;
            int lessOfLimitAmount = 0;
            
            if (_height.currentHeight < yLimit)
            {
                if (_isShow)
                    Hide();
                return;
            }
            
            for (var x = 0; x < _matrix.wight; x++)
            for (var z = 0; z < _matrix.wight; z++)
            {
                int y = _matrix.MinHeightInCoordinates(x, z);

                if (y < yLimit) lessOfLimitAmount++;
                
                if (y < yLimit && lessOfLimitAmount > maxLessOfLimit && !_isShow)
                    return;
                if (y > yLimit)
                    outOfLimitAmount++;
            }

            if (outOfLimitAmount >= needOutOfLimitAmount && !_isShow)
                Show();
            else if(outOfLimitAmount < needOutOfLimitAmount && _isShow)
                Hide();
        }

        private void Show()
        {
            _isShow = true;
            _offer.gameObject.SetActive(true);
            _show.Rewind();
            _show.Play();
            
            _bombIcon.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one * 1.2f).SetLoops(-1,LoopType.Yoyo);

        }

        private void Hide()
        {
            _isShow = false;
            _hide.Rewind();
            _hide.Play();
        }

        public void Apply()
        {
            //todo ads

            _betweenOffersStepsCurrent = 0;
            _inOneGameCurrent++;
            Hide();
        }
        public void Clear()
        {
            _offer.gameObject.SetActive(false);
            _isShow = false;
           
            _betweenOffersStepsCurrent = _betweenOffersSteps + 1;
            _inOneGameCurrent = 0;
        }
    }
}