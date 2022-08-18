using System;
using System.Collections.Generic;
using DG.Tweening;
using Helper.Patterns;
using IntegerExtension;
using Script.PlayerProfile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Home
{
    public class CurrencyLabel : MonoBehaviour
    {
        [SerializeField] private bool _linkWithWallet = true;
        
        [SerializeField] private Currency _currency;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private float _animationDuration;
        [SerializeField] private RectTransform _currencyIcon;
        
        private Sequence _changeAnimation;
        private int currentInt, endInt;
        
        // money animation
        [SerializeField] private GameObject _iconPrefab;
        [SerializeField] private float _delayBetweenIcon;
        [SerializeField] private float _timeMove;
        [SerializeField] private RectTransform _startAnimationPoint;

        [SerializeField] private Transform poolParent;
        private Pool<GameObject> _iconPool;
        private List<GameObject> _iconsAnimation;
        


        private void Start()
        {
            if (_linkWithWallet)
            {
                currentInt = PlayerSaveProfile.instance.GetCurrencyAmount(_currency);
                _text.text = currentInt.ToString();
           
                PlayerSaveProfile.instance.onCurrencyAmountChanged += OnCurrencyAmountChanged;
            }

            if (_currency == Currency.coin)
                _animationDuration *= 2;
            
            _changeAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _changeAnimation.Append(_currencyIcon.DOScale(1.3f, _animationDuration).From(1f))
                .Append(_currencyIcon.DOScale(1f, _animationDuration/2).From(1.3f));
            _changeAnimation.Complete();

            _iconPool = new Pool<GameObject>(_iconPrefab, poolParent);
            _iconsAnimation = new List<GameObject>();
        }

        public void SetCurrencyAmount(int amount)
        {
            currentInt = amount;
            _text.text = amount.ToString();
        }
        
        public void OnCurrencyAmountChanged(Currency type, int amount)
        {
            if (type == _currency)
            {
                endInt = amount;
                _text.text = amount.ToString();

                if (amount > currentInt)
                    AddCurrencyAnimation(amount - currentInt);
                
                _changeAnimation.Rewind();
                _changeAnimation.Play();
                DOTween.To(x => _text.text = ((int)x).ToString(), currentInt, endInt, _animationDuration)
                    .SetEase(Ease.OutCirc)
                    .SetAutoKill(true).OnComplete(() =>
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(_text.GetComponent<RectTransform>());
                        currentInt = endInt;
                    });
            }
        }
        
        private void OnDestroy()
        {
            PlayerSaveProfile.instance.onCurrencyAmountChanged -= OnCurrencyAmountChanged;
        }

        // Animation 
        public void AddCurrencyAnimation(int difference)
        {
            _delayBetweenIcon = _animationDuration / difference;
            for (int i = 0; i < difference; i++)
            {
                Invoke(nameof(SpawnIcon), i*_delayBetweenIcon);
            }
            Invoke(nameof(ResetAllIcon), difference*_delayBetweenIcon + _timeMove);
        }

        public void SpawnIcon()
        {
            var icon = _iconPool.Pop();
            icon.GetComponent<RectTransform>().DOAnchorPos(_currencyIcon.anchoredPosition, _timeMove)
                .From(_startAnimationPoint.anchoredPosition);
            _iconsAnimation.Add(icon);
        }

        public void ResetAllIcon()
        {
            foreach (var a in _iconsAnimation)
            {
                _iconPool.Push(a);
            }
            _iconsAnimation.Clear();
        }

    }
}