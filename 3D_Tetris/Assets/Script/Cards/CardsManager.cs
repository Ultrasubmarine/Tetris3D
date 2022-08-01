using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Cards
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _hideElements; // when unlockPanel opened
        [SerializeField] private float _hideTime;
        
        [SerializeField] private UnlockCardPanel _unlockCardPanel;
        [SerializeField] private CardsList _cards;

        [SerializeField] private Button _openBtn;
        
        private int currentCard = 0;

        private void Start()
        {
            Load();
            
            _openBtn.onClick.AddListener(Open);
            _unlockCardPanel.closeBtn.onClick.AddListener(CloseUnlockPanel);
            _unlockCardPanel.gameObject.SetActive(false);
        }

        public void Load()
        {
            _unlockCardPanel.Load(new List<int>(), _cards.cards[currentCard]);
        }

        public void Open()
        {
            OpenUnlockPanel();
        }

        public void OpenUnlockPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(0, _hideTime).From(1);
            }
            _unlockCardPanel.gameObject.SetActive(true);
            _unlockCardPanel.canvasGroup.DOFade(1, _hideTime * 1.5f).From(0);
            
            _unlockCardPanel.Open();
        }

        public void CloseUnlockPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOFade(1, _hideTime* 1.5f).From(0);
            }
            
            _unlockCardPanel.canvasGroup.DOFade(0, _hideTime).From(1)
                .OnComplete(()=>_unlockCardPanel.gameObject.SetActive(false));
            
        }
    }
}