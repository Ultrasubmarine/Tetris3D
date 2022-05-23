using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class InventoryUI: MonoBehaviour
    {
        private Generator _generator;
        
        private bool _isShow = false;
        [SerializeField] private BottomPanelAnimation _panel;

        //big bomb
        [SerializeField] private TextMeshProUGUI _bombAmountText;
        [SerializeField] private RectTransform _bombRect;
        private Sequence _addBomb;

        [SerializeField] private float _time = 0.5f;
        private Sequence _applyBomb;
        
        
        private void Start()
        {
            _generator = RealizationBox.Instance.generator;
            
            SetBombAmountText();
  
            _applyBomb = DOTween.Sequence().SetAutoKill(false).Pause();
            _applyBomb.Append(_bombRect.DOScale(Vector3.one * 1.3f, 0.3f).From(Vector3.one).OnComplete(()=>SetBombAmountText()))
                .Append(_bombRect.DOScale(Vector3.one, _time / 2))
                .OnComplete(() => UpdateVisibility());
                
            UpdateVisibility();
        }

        public void UseBigBomb()
        {
            _applyBomb.Rewind();
            _applyBomb.Play();

            if (PlayerSaveProfile.instance._bombAmount > 0 && _generator.nextElement.type != ElementType.bigBomb)
            {
                PlayerSaveProfile.instance.SetBombAmount(PlayerSaveProfile.instance._bombAmount - 1);
                RealizationBox.Instance.generator.SetNextAsBigBomb();
            }
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

    }
}