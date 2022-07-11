using System;
using DG.Tweening;
using IntegerExtension;
using Script.PlayerProfile;
using TMPro;
using UnityEngine;

namespace Script.Home
{
    public class CurrencyLabel : MonoBehaviour
    {
        [SerializeField] private Currency _currency;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private float _animationDuration;
        [SerializeField] private RectTransform _currencyIcon;
        
        
        private Sequence _changeAnimation;
        private int currentInt, endInt;
        
        private void Start()
        {
           currentInt = PlayerSaveProfile.instance.GetCurrencyAmount(_currency);
           _text.text = currentInt.ToString();
           
            PlayerSaveProfile.instance.onCurrencyAmountChanged += OnCurrencyAmountChanged;
            
            _changeAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _changeAnimation.Append(_currencyIcon.DOScale(1.3f, _animationDuration).From(1f))
                .Append(_currencyIcon.DOScale(1f, _animationDuration/2).From(1.3f));
            _changeAnimation.Complete();

        }

        public void OnCurrencyAmountChanged(Currency type, int amount)
        {
            if (type == _currency)
            {
                endInt = amount;
                _text.text = amount.ToString();

                _changeAnimation.Rewind();
                _changeAnimation.Play();
                DOTween.To(x => _text.text = ((int)x).ToString(), currentInt, endInt, _animationDuration)
                    .SetEase(Ease.OutCirc)
                    .SetAutoKill(true).OnComplete(() => currentInt = endInt);
            }
        }
        
        private void OnDestroy()
        {
            PlayerSaveProfile.instance.onCurrencyAmountChanged -= OnCurrencyAmountChanged;
        }
        
        
    }
}