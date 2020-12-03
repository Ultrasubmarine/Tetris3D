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

        public void Show()
        {
            _hidingMask.DOFade(0, _speedShow);
            _content.DOFade(1, _speedShow);
        }

        public void Hide()
        {
            _content.alpha = 0;
            _hidingMask.alpha = 1;
        }
    }
}