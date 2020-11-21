using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Comics
{
    [RequireComponent(typeof(TapsEvents))]
    public class Comics : MonoBehaviour
    {
        private TapsEvents _tapsEvents;

        public event Action OnSingleTap;
        public event Action OnDoubleTap;

        [SerializeField] private List<Page> _pages;

        private void Start()
        {
            _tapsEvents = GetComponent<TapsEvents>();
            
            _tapsEvents.OnSingleTap += () => OnSingleTap.Invoke();
            _tapsEvents.OnDoubleTap += () => OnDoubleTap.Invoke();

            foreach (var page in _pages)
            {
                page.Initialize(this);
            }
        }
    }
}