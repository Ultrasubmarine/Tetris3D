using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.FontsAnimations
{
    [Serializable]
    public struct FontsAnimation
    {
        public int combo;
        public List<CanvasGroup> txt;

    }

    public class FontsCongratsManager : MonoBehaviour
    {
        private int combo = 0;
        private Dictionary<int, List<Sequence>> _animation;

        [SerializeField] private List<FontsAnimation> _congrats;

        [SerializeField] private float firstMove = 40;
        [SerializeField] private float firstMoveTime = 1;
        
        [SerializeField] private float reverseMove;
        [SerializeField] private float reverseMoveTime;
        
        [SerializeField] private float wait = 0.2f;
        
        [SerializeField] private float dissapeadScale;
        [SerializeField] private float dissapeadScaleTime;
        private void Start()
        {
            _animation = new Dictionary<int, List<Sequence>>();

            
            for (int i = 0; i < _congrats.Count; i++)
            {
                _animation[_congrats[i].combo] = new List<Sequence>();

                foreach (var txt in _congrats[i].txt)
                {
                    Sequence s = DOTween.Sequence().SetAutoKill(false).Pause();
                    
                    var rt = txt.gameObject.GetComponent<RectTransform>();
                    s.Append(rt.DOLocalMoveY(rt.localPosition.y + firstMove, firstMoveTime).From(rt.localPosition.y))
                        .Join(txt.DOFade(1, firstMoveTime / 2).From(0, false))
                        .Append(rt.DOLocalMoveY(rt.localPosition.y + firstMove - reverseMove, reverseMoveTime))
                        .AppendInterval(wait)
                        .Append(txt.DOFade(0, dissapeadScaleTime))
                        .Join(rt.DOScale(Vector3.one * dissapeadScale, dissapeadScaleTime))
                        .OnComplete(() => rt.localScale = Vector3.one);
                    _animation[_congrats[i].combo].Add(s);
                }
            }

            RealizationBox.Instance.FSM.onStateChange += OnFSMStateChange;
            RealizationBox.Instance.matrix.OnDestroyLayer += PlayAnimation;
        }

        public void PlayAnimation(int layer)
        {
            combo++;
            if (!_animation.ContainsKey(combo))
                return;
            
            var animation = _animation[combo][Random.Range(0, _animation[combo].Count-1)];
            
            animation.Rewind();
            animation.Play();
        }

        public void OnFSMStateChange(TetrisState state)
        {
            if (state == TetrisState.GenerateElement)
                combo = 0;
        }
    }
}