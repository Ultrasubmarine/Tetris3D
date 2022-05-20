using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class InventoryUI: MonoBehaviour
    {
        private bool _isShow = false;
        [SerializeField] private BottomPanelAnimation _panel;

        [SerializeField] private bool gamePlayState; // always shsow
        
        //big bomb
        [SerializeField] private TextMeshProUGUI _bombAmountText;
        [SerializeField] private RectTransform _bombRect;
        private Sequence _changeAmountBombAnimation;

        [SerializeField] private float _time = 0.5f;
        private Sequence _applyBomb;
        
        
        private void Start()
        {
            
            if (!gamePlayState)
            {
                PlayerSaveProfile.instance.onBombAmountChange += AddBigBombInInventory;
            }
            else
            {
             //   PlayerSaveProfile.instance.onBombAmountChange += UpdateVisibility;
                
                _applyBomb = DOTween.Sequence().SetAutoKill(false).Pause();
                _applyBomb.Append(_bombRect.DOScale(Vector3.one * 1.3f, 0.3f).From(Vector3.one))
                    .Append(_bombRect.DOScale(Vector3.one, _time / 2))
                    .OnComplete(() => UpdateVisibility());
            }
            
            if (gamePlayState)
            {
                UpdateVisibility();
            }
        }

        public void UseBigBomb()
        {
            _applyBomb.Rewind();
            _applyBomb.Play();

            if (PlayerSaveProfile.instance._bombAmount > 0)
            {
                PlayerSaveProfile.instance.SetBombAmount(PlayerSaveProfile.instance._bombAmount - 1);
                RealizationBox.Instance.generator.SetNextAsBigBomb();
            }
        }
        
        public void AddBigBombInInventory(int amount)
        {
            
        }

        
        public void UpdateVisibility(int amount = 0)
        {
            SetBombAmountText();
            
            if (PlayerSaveProfile.instance._bombAmount > 0 && !_isShow)
            {
                _isShow = true;
                _panel.Show();
            }
            else if (PlayerSaveProfile.instance._bombAmount <= 0 && _isShow)
            {
                _isShow = false;
                _panel.Hide();
            }
        }
        
        public void SetBombAmountText()
        {
            _bombAmountText.text = "<size=60%>x</size=70%>" + PlayerSaveProfile.instance._bombAmount;
        }
        
        private void OnDestroy()
        {
            if (!gamePlayState)
            {
                PlayerSaveProfile.instance.onBombAmountChange -= AddBigBombInInventory;
            }
            else
            {
           //     PlayerSaveProfile.instance.onBombAmountChange -= UpdateVisibility;
            }
        }

    }
}