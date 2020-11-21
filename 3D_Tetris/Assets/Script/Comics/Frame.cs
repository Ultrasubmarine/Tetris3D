using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Comics
{
    public class Frame : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _content;
        [SerializeField] private CanvasGroup _hidingMask;

        [SerializeField] private float _speedShow = 0.2f;
        [SerializeField] private float _speedFastShow = 0.1f;
        
        public bool isHide { get; private set; } = true;
        private Action OnShow;

        public void Show()
        {
            _hidingMask.DOFade(0, _speedShow);
            _content.DOFade(1, _speedShow).OnComplete(OnFinish);
        }

        private void OnFinish()
        {
            isHide = false;
            OnShow.Invoke();
        }

        public void Hide()
        {
            _content.alpha = 0;
            _hidingMask.alpha = 1;
        }
    }
}