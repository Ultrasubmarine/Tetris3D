using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.Cards
{
    public class UnlockCardPanel: MonoBehaviour
    {
        public CanvasGroup canvasGroup { get; private set; }
        public Button closeBtn => _closeBtn;
        [SerializeField] private Button _closeBtn;
        
        [SerializeField] private Image _image;
        [SerializeField] private Button _unlockBtn;

        [SerializeField] private List<CanvasGroup> _puzzle;
        private List<CanvasGroup> _showPuzzle;

        [SerializeField] private float _timeAlphaOpen;
        [SerializeField] private float _timeAlphaUnlock;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            _showPuzzle = new List<CanvasGroup>();
            _unlockBtn.onClick.AddListener(Unlock);
            
        }
        
        public void Load(List<int> showIndexes, Sprite sprite)
        {
            _showPuzzle.Clear();
            foreach (var i in showIndexes)
            {
                _showPuzzle.Add(_puzzle[i]);
            }

            _image.sprite = sprite;
        }

        public void OpenUnlocked()
        {
            foreach (var s in _showPuzzle)
            {
                s.DOFade(0, _timeAlphaOpen);
            }
            _image.gameObject.SetActive(true);
        }

        public void HideAll()
        {
            foreach (var p in _puzzle)
            {
                p.alpha = 1;
                p.DOKill();
            }
            _image.gameObject.SetActive(false);
        }
        
        public void Unlock()
        {
            if(_showPuzzle.Count == _puzzle.Count)
                return;
            
            var lockedP = _puzzle.Except(_showPuzzle).ToArray();

            var unlocked = lockedP[Random.Range(0, lockedP.Count())];
            
            _showPuzzle.Add(unlocked);
            unlocked.DOFade(0, _timeAlphaUnlock);

            var index = _puzzle.IndexOf(unlocked);
            //save index;
        }
        
    }
}