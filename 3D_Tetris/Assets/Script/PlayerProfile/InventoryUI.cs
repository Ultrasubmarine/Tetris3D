using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class InventoryUI: MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panel;

        [SerializeField] private bool gamePlayState; // always shsow
        
        //big bomb
        [SerializeField] private TextMeshProUGUI _bombAmountText;
        [SerializeField] private RectTransform _bombRect;
        private Sequence _changeAmountBombAnimation;
        
        private void Start()
        {
            
        }

        public void UseBigBomb()
        {
            
        }

        public void AddBigBombInInventory()
        {
            
        }
    }
}