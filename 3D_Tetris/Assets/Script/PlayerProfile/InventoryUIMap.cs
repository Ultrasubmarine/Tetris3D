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

        //big bomb
        [SerializeField] private TextMeshProUGUI _bombAmountText;
        [SerializeField] private RectTransform _bombRect;
        private Sequence _addBomb;

        [SerializeField] private float _time = 0.5f;


        private void Start()
        {
            SetBombAmountText();
        
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

        public void AddBigBombInInventory(int amount)
        {
            _panel.Show();
        }

        public void SetBombAmountText()
        {
            _bombAmountText.text = "<size=60%>x</size=70%>" + PlayerSaveProfile.instance._bombAmount;
        }
        
        private void OnDestroy()
        {
            PlayerSaveProfile.instance.onBombAmountChange -= AddBigBombInInventory;
        }
    }
}