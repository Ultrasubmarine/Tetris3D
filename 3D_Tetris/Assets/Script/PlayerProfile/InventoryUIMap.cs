using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class InventoryUIMap : MonoBehaviour
    {
         private bool _isShow = false;
        [SerializeField] private BottomPanelAnimation _panel;

        [SerializeField] private bool gamePlayState; // always shsow
        
        //big bomb
        [SerializeField] private TextMeshProUGUI _bombAmountText;
        [SerializeField] private RectTransform _bombRect;
        private Sequence _addBomb;

        [SerializeField] private float _time = 0.5f;
        private Sequence _applyBomb;
        
        
        private void Start()
        {
            SetBombAmountText();
            if (!gamePlayState)
            {
                PlayerSaveProfile.instance.onBombAmountChange += AddBigBombInInventory;
                
                // todo fix this ugly code 
                _panel.onShowEnded += () =>
                {
                    _addBomb.Rewind();
                    _addBomb.Play();
                };
                
                _addBomb= DOTween.Sequence().SetAutoKill(false).Pause();
                _addBomb.AppendInterval(0.7f)
                    .Append(_bombRect.DOScale(Vector3.one * 1.3f, 0.3f).From(Vector3.one).OnComplete(()=>SetBombAmountText()))
                    .Append(_bombRect.DOScale(Vector3.one, _time / 2))
                    .AppendInterval(0.2f)
                    .OnComplete(() => _panel.Hide());
            }
            else
            {
             //   PlayerSaveProfile.instance.onBombAmountChange += UpdateVisibility;
                
                _applyBomb = DOTween.Sequence().SetAutoKill(false).Pause();
                _applyBomb.Append(_bombRect.DOScale(Vector3.one * 1.3f, 0.3f).From(Vector3.one).OnComplete(()=>SetBombAmountText()))
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
            _panel.Show();
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