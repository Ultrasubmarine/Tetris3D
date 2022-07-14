using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.GameLogic.Stars
{
    public class StarUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private StarsManager _starsManager;

        private void Start()
        {
            _starsManager = RealizationBox.Instance.starsManager;

            if (!RealizationBox.Instance.gameManager.infinity)
            {
                _starsManager.OnUpdatedCollectingStars += OnUpdateScore;
                RealizationBox.Instance.starUIAnimation.OnUpdateStartScoreText += OnUpdateScore;
                RealizationBox.Instance.starUIAnimation.OnAnimationEnd += OnUpdateScore;
                _text.text = _starsManager.collectedStars + " / " + _starsManager.neededStars;
            }
            else
            {
                _starsManager.OnUpdatedCollectingStars += OnUpdateInfinityScore;
                RealizationBox.Instance.starUIAnimation.OnUpdateStartScoreText += OnUpdateInfinityScore;
                RealizationBox.Instance.starUIAnimation.OnAnimationEnd += OnUpdateInfinityScore;
                _text.text = _starsManager.collectedStars.ToString();
            }
        }

        public void OnUpdateScore()
        {
            _text.text = _starsManager.collectedStars + " / " + _starsManager.neededStars;
        }

        public void OnUpdateInfinityScore()
        {
            _text.text = _starsManager.collectedStars.ToString();
        }
    }
}