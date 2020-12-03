using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.Comics
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private List<Frame> _frames;
        [SerializeField] private bool _showFirst = true;

        [SerializeField] private float _timeHiding = 0.2f;
        
        private int _currentIndex = -1;
        private CanvasGroup _canvasGroup;
        
        private RectTransform _rectTransform;
        
        // for drag
        private Vector2 _startFingerPosition;
        private Comics _comics;

        public Action OnFinishedRead;
        
        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            foreach (var frame in _frames)
            {
                frame.Hide();
            }

            if (_frames.Count > 0 && _showFirst)
                ShowOne();
        }

        public void Initialize(Comics comics)
        {
            _comics = comics;
            comics.OnSingleTap += ShowOne;
            comics.OnDoubleTap += ShowAll;
        }

        private void ShowOne()
        {
            if (_currentIndex < _frames.Count - 1)
            {
                _frames[++_currentIndex].Show();
                
                if(_currentIndex >= _frames.Count - 1)
                    OnFinish();
            }
        }

        private void ShowAll()
        {
            foreach (var frame in _frames)
            {
                frame.Show();
            }
            OnFinish();
        }

        private void OnFinish()
        {
            _canvasGroup.blocksRaycasts = true;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startFingerPosition = eventData.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition = eventData.position - _startFingerPosition ; 
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.DOFade(0, _timeHiding).OnComplete(() =>
            {
                _comics.OnSingleTap -= ShowOne;
                _comics.OnDoubleTap -= ShowAll;
                OnFinishedRead.Invoke();
                this.gameObject.SetActive(false);
            });
        }
    }
}