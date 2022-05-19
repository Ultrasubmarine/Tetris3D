using System;
using DG.Tweening;
using Script.GameLogic.Bomb;
using Script.GameLogic.TetrisElement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Script.Offers
{
    public class BigBombGamePlayOffer: MonoBehaviour
    {
        private PlaneMatrix _matrix;
        private HeightHandler _height;
        private Generator _generator;
        private ElementData _elementData;
        private ChangeNewElementToBomb _changeNewElementToBomb;
        
        [SerializeField] private RectTransform _bombIcon;
        [SerializeField] private CanvasGroup _offer;
        
        public int betweenOffersSteps = 3;
        public int inOneGameMax = 2;
        
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
            _generator = RealizationBox.Instance.generator;
            _elementData = ElementData.Instance;
            _changeNewElementToBomb = RealizationBox.Instance.changeNewElementToBomb;

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

            Show();
        }

        public void CheckShowOffer()
        {
            if (_inOneGameCurrent > inOneGameMax)
                return;

            if (_betweenOffersStepsCurrent < betweenOffersSteps)
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
            // _isShow = false;
            // _hide.Rewind();
            // _hide.Play();
        }

        public void Apply()
        {
            //todo ads

            _betweenOffersStepsCurrent = 0;
            _inOneGameCurrent++;
            Hide();
            
            if(Equals(_elementData.newElement, null))
                _generator.SetNextAsBigBomb();
            else
            {
                _changeNewElementToBomb.ChangeToBomb(true);
            }
        }
        public void Clear()
        {
            _offer.gameObject.SetActive(false);
            _isShow = false;
           
            _betweenOffersStepsCurrent = betweenOffersSteps + 1;
            _inOneGameCurrent = 0;
        }
    }
}