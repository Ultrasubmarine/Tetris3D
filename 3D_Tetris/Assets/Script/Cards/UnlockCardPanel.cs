using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Script.PlayerProfile;
using TMPro;
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
        [SerializeField] private CanvasGroup _unlockBtnCanvasGroup;
        [SerializeField] private Transform _mask;
        
        [SerializeField] private List<CanvasGroup> _puzzle;
        private List<CanvasGroup> _showPuzzle;

        [SerializeField] private float _timeAlphaOpen;
        [SerializeField] private float _timeAlphaUnlock;
        
        [SerializeField] private RectTransform _needTextRect;
        [SerializeField] private float _needTextMoveTime;
        [SerializeField] private int _needTextMoveDelta;
        private Sequence _needAnimation;
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int step = 10;
        private int cost;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            _showPuzzle = new List<CanvasGroup>();
            
            _unlockBtn.onClick.AddListener(Unlock);
            
            _needAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _needAnimation.Append(_needTextRect.DOAnchorPosY(_needTextMoveDelta + 155, _needTextMoveTime)
                    .From(Vector2.up * 155).OnPlay(() => _needTextRect.GetComponent<CanvasGroup>().alpha = 1))
                .Append(_needTextRect.GetComponent<CanvasGroup>().DOFade(0, 0.4f).From(1));
            
            _needAnimation.Complete();
        }
        
        public void Load(List<int> showIndexes, Sprite sprite)
        {
            _showPuzzle.Clear();
            foreach (var i in showIndexes)
            {
                _showPuzzle.Add(_puzzle[i]);
            }

            _image.sprite = sprite;
            UpdatePuzzleCost();
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

            if (!CanUnlock())
                return;
            
            var lockedP = _puzzle.Except(_showPuzzle).ToArray();

            var unlocked = lockedP[Random.Range(0, lockedP.Count())];
            
            _showPuzzle.Add(unlocked);
            unlocked.DOFade(0, _timeAlphaUnlock);

            var index = _puzzle.IndexOf(unlocked);
            PlayerSaveProfile.instance.AddUnlockCardPart(index);
            UpdatePuzzleCost();
            
            if (_showPuzzle.Count == _puzzle.Count)
            {
                _unlockBtnCanvasGroup.DOFade(0, 0.2f).From(1);
                Invoke(nameof(FullUnlock),_timeAlphaUnlock);
                
                PlayerSaveProfile.instance.ResetUnlockedCardParts();
                PlayerSaveProfile.instance.IncrementCurrentCard();
            }
               
            //save index;
        }

        private bool CanUnlock()
        {
            if (PlayerSaveProfile.instance.ChangeCurrencyAmount(Currency.stars, -cost))
            {
                return true;
            }

            _needAnimation.Rewind();
            _needAnimation.Play();
            return false;
            // return true;
            //todoMoney
        }

        public void FullUnlock()
        {
            _mask.DOScale(1.3f, 0.4f);
        }

        private void OnDisable()
        {
            _mask.localScale = Vector3.one;
            _unlockBtnCanvasGroup.alpha = 1;
        }

        private void UpdatePuzzleCost()
        {
            cost = step * (_showPuzzle.Count+1);
            _text.text = cost.ToString();
        }
    }
}