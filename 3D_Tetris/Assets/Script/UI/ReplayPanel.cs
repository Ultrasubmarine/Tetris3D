using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Script.UI
{
    public class ReplayPanel :MonoBehaviour
    {
        
        [SerializeField] private float speedShow = 0.3f;
        [SerializeField] private float speedDissapear = 0.1f;

        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private CanvasGroup _pauseButton;
        
        [SerializeField] private List<GameObject> _adsObjects;
        
        private void OnEnable()
        {
            AdsSet();
            _canvas.DOFade(1, speedShow ).From(0);
            _pauseButton.DOFade(0, speedShow /2).From(1).OnComplete(()=> { _pauseButton.gameObject.SetActive(false);});
        }

        public void Hide()
        {
            _canvas.DOFade(0, speedDissapear).From(1).OnComplete(()=> { this.gameObject.SetActive(false);});
            _pauseButton.gameObject.SetActive(true);
            _pauseButton.DOFade(1, speedDissapear/2).From(0);
        }
        
        public void AdsSet()
        {
            foreach (var c in _adsObjects)
            {
                c.SetActive(AdsManager.instance.isAds);
            }
        }
        
    }
}