using System;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Script.Home
{
    public class ToglePanel: MonoBehaviour
    {
        [SerializeField] private RectTransform _animationButton;
        [SerializeField] private CanvasGroup _animationTextCanvas;
        [SerializeField] private Text _text;
        
        [SerializeField] private Button _switchButton;

        [SerializeField] private float _time;
        private Sequence _leftAnimation, _rightAnimation;

        [SerializeField] private CanvasGroup _leftCanvas;
        [SerializeField] private CanvasGroup _rightCanvas;


        bool _isLeft;
        
        private void Start()
        {
            _switchButton.onClick.AddListener(OnSwitch);
            
            _leftAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _leftAnimation
                .Append(_animationTextCanvas.DOFade(0f, _time / 4).From(1f))
                .Join(_rightCanvas.DOFade(0f, _time / 4))
                .Append(_animationButton.DOAnchorPosX(-_animationButton.sizeDelta.x/2, _time / 2)
                    .From(Vector2.right * _animationButton.sizeDelta.x/2)
                    .OnComplete(() => _text.text = "home"))
                .Join(_leftCanvas.DOFade(1f, _time / 3).OnComplete(() => _leftCanvas.blocksRaycasts = true))
                .Append(_animationTextCanvas.DOFade(1f, _time / 4).From(0f));
            
            _rightAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _rightAnimation
                .Append(_animationTextCanvas.DOFade(0f, _time / 4).From(1f))
                .Join(_leftCanvas.DOFade(0f, _time / 4))
                .Append(_animationButton.DOAnchorPosX(_animationButton.sizeDelta.x/2, _time / 2)
                    .From(Vector2.right * -_animationButton.sizeDelta.x/2)
                    .OnComplete(() => _text.text = "cards"))
                .Join(_rightCanvas.DOFade(1f, _time / 3).OnComplete(() => _rightCanvas.blocksRaycasts = true))
                .Append(_animationTextCanvas.DOFade(1f, _time / 4).From(0f));

            _isLeft = true;
            _leftAnimation.Complete();
        }

        public void OnSwitch()
        {
            if (_isLeft)
            {
                _leftAnimation.Complete();
                _leftCanvas.blocksRaycasts = false;
               
                _rightAnimation.Rewind();
                _rightAnimation.Play();
                
            }
            else
            {
                _rightAnimation.Complete();
                _rightCanvas.blocksRaycasts = false;
                
                _leftAnimation.Rewind();
                _leftAnimation.Play();
            }
            _isLeft = !_isLeft;
        }
    }
}