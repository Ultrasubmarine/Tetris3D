using System;
using DG.Tweening;
using Script.GameLogic.Bomb;
using Script.GameLogic.TetrisElement;
using Script.PlayerProfile;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

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
        [SerializeField] private CanvasGroup _offerButtonCanvas;
        [SerializeField] private Button _offerButton;
        
        [SerializeField] private CanvasGroup _offerExtraPanel;
        [SerializeField] private Button _extraPanelCloseButton;
        [SerializeField] private Button _extraPanelAdsButton;
        
        public int betweenOffersSteps = 3;
        public int inOneGameMax = 2;
        
        private int _betweenOffersStepsCurrent;
        private int _inOneGameCurrent;
        
        [SerializeField] private int needOutOfLimitAmount = 5;
        [SerializeField] private int maxLessOfLimit = 1;
        
        private bool _isShow = false;
        
        //Animation
        private Sequence _showBtn, _hideBtn;
        [SerializeField] private float _yMove = -100;
        [SerializeField] private float _time = 0.5f;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _height = RealizationBox.Instance.haightHandler;
            _generator = RealizationBox.Instance.generator;
            _elementData = ElementData.Instance;
            _changeNewElementToBomb = RealizationBox.Instance.changeNewElementToBomb;

            RealizationBox.Instance.FSM.AddListener(TetrisState.GenerateElement, CheckShowOfferBtn);
            RealizationBox.Instance.FSM.AddListener(TetrisState.LoseGame, HideBtn);
            RealizationBox.Instance.FSM.AddListener(TetrisState.WinGame, HideBtn);

            var rectTransform = _offerButtonCanvas.GetComponent<RectTransform>();
            
            _showBtn = DOTween.Sequence().SetAutoKill(false).Pause();
            _showBtn.Append(_offerButtonCanvas.DOFade(1, _time / 2).From(0))
                .Join(rectTransform.DOAnchorPosY(0, _time / 2).From(Vector2.up * _yMove));

            _hideBtn = DOTween.Sequence().SetAutoKill(false).Pause();
            _hideBtn.Append(_offerButtonCanvas.DOFade(0, _time / 2).From(1))
                .Join(rectTransform.DOAnchorPosY(_yMove, _time / 2).From(Vector2.zero)).OnComplete( () =>
                {
                    _bombIcon.DOKill();
                    _offerButtonCanvas.gameObject.SetActive(false);
                    _hideBtn.Rewind();
                });
            
            Clear();

            _offerButton.onClick.AddListener(OnOfferButtonClick);
            
            _extraPanelCloseButton.onClick.AddListener(HideExtraPanel);
            _extraPanelAdsButton.onClick.AddListener(OnAdsButtonClick);
         //   HideExtraPanel();
            //  Show();
        }

        public void CheckShowOfferBtn()
        {
            if (_inOneGameCurrent > inOneGameMax)
                return;

            if (PlayerSaveProfile.instance._bombAmount > 0)
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
                    HideBtn();
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
                ShowBtn();
            else if(outOfLimitAmount < needOutOfLimitAmount && _isShow)
                HideBtn();
        }

        public void ShowBtn()
        {
            _isShow = true;
            _offerButtonCanvas.gameObject.SetActive(true);
            _showBtn.Rewind();
            _showBtn.Play();
            
            _bombIcon.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one * 1.2f).SetLoops(-1,LoopType.Yoyo);
            RealizationBox.Instance.slowManager.SetOfferSlow(true);
        }

        private void HideBtn()
        {
            _isShow = false;
            _hideBtn.Rewind();
            _hideBtn.Play();

            RealizationBox.Instance.slowManager.SetOfferSlow(false);
        }


        private void ShowExtraPanel()
        {
            _offerExtraPanel.gameObject.SetActive(true);
            _offerExtraPanel.DOFade(1, _time / 2).From(0);
            RealizationBox.Instance.influenceManager.enabled = false;
        }

        public void HideExtraPanel()
        {
            _offerExtraPanel.DOFade(0, _time / 2).From(1)
                .OnComplete(()=>
                {
                    _offerExtraPanel.gameObject.SetActive(false);
                    RealizationBox.Instance.influenceManager.enabled = true;
                });
            
        }


        public void OnOfferButtonClick()
        {
            if (true) // currency < 1000
                ShowExtraPanel();
            else
                MakeBigBomb();
        }

        public void OnAdsButtonClick()
        {
            //todo ads
            HideExtraPanel();
            MakeBigBomb();
        }
        
        public void MakeBigBomb()
        {
           
            _betweenOffersStepsCurrent = 0;
            _inOneGameCurrent++;
            HideBtn();
            
            if(Equals(_elementData.newElement, null))
                _generator.SetNextAsBigBomb();
            else
            {
                _changeNewElementToBomb.ChangeToBomb(true);
            }
        }
        public void Clear()
        {
            _offerButtonCanvas.gameObject.SetActive(false);
            _isShow = false;
           
            _betweenOffersStepsCurrent = betweenOffersSteps + 1;
            _inOneGameCurrent = 0;
            
            _offerExtraPanel.gameObject.SetActive(false);
        }
    }
}