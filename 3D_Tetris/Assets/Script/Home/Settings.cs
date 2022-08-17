using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Script.Home
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _hideElements;
        [SerializeField] private CanvasGroup _settingsPanel;
        
        public void OpenSettingsPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOKill();
                h.DOFade(0, 0.1f);
                h.interactable = false;
            }

            _settingsPanel.interactable = true;
            _settingsPanel.blocksRaycasts = true;
            _settingsPanel.DOKill();
            _settingsPanel.DOFade(1, 0.3f);
        }
        
        public void CloseSettingsPanel()
        {
            foreach (var h in _hideElements)
            {
                h.DOKill();
                h.DOFade(1, 0.3f);
                h.interactable = true;
            }

            _settingsPanel.interactable = false;
            _settingsPanel.blocksRaycasts = false;
            _settingsPanel.DOKill();
            _settingsPanel.DOFade(0, 0.1f);
        }

    }
}