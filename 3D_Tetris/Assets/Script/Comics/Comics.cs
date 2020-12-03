using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Comics
{
    [RequireComponent(typeof(TapsEvents))]
    public class Comics : MonoBehaviour
    {
        private TapsEvents _tapsEvents;

        public event Action OnSingleTap;
        public event Action OnDoubleTap;

        [SerializeField] private List<Page> _pages;
        [SerializeField] private Image _background;
        [SerializeField] private float _timeAlphaBackground;
        [SerializeField] private float _timeWaitAlphaBackground;
        
        private int _currentIndex = -1;
        
        private void Start()
        {
            _tapsEvents = GetComponent<TapsEvents>();
            
            _tapsEvents.OnSingleTap += () => OnSingleTap.Invoke();
            _tapsEvents.OnDoubleTap += () => OnDoubleTap.Invoke();

            if(_pages.Count > 0)
                SetPage();
                
        }

        public void SetPage()
        {
            if (_currentIndex < _pages.Count - 1)
            {
                _pages[++_currentIndex].Initialize(this);
                _pages[_currentIndex].OnFinishedRead += SetPage;
            }
            else
            {
                Invoke("FadeBG", _timeWaitAlphaBackground);
            }
        }
        
        void FadeBG()
        {
            _background.DOFade(0, _timeAlphaBackground).OnComplete( () => gameObject.SetActive(false));
        }
    }
}