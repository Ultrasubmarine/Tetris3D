using System.Collections.Generic;
using UnityEngine;

namespace Script.Comics
{
    public class Page : MonoBehaviour
    {
        [SerializeField] private List<Frame> _frames;
        [SerializeField] private bool _showFirst = true;
        
        private int _currentIndex = -1;

        private void Start()
        {
            foreach (var frame in _frames)
            {
                frame.Hide();
            }

            if (_frames.Count > 0 && _showFirst)
            {
                _frames[++_currentIndex].Show(); 
            }
        }

        public void Initialize(Comics comics)
        {
            comics.OnSingleTap += ShowOne;
            comics.OnDoubleTap += ShowAll;
        }

        private void ShowOne()
        {
            if (_currentIndex < _frames.Count - 1)
            {
                _frames[++_currentIndex].Show();
            }
        }

        private void ShowAll()
        {
            foreach (var frame in _frames)
            {
                frame.Show();
            }
        }
    }
}