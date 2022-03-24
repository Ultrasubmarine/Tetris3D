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
            _starsManager.OnUpdatedCollectingStars += OnUpdateScore;
            RealizationBox.Instance.starUIAnimation.OnUpdateStartScoreText += OnUpdateScore;
            _text.text = _starsManager.collectedStars + " / " + _starsManager.neededStars;
        }

        public void OnUpdateScore()
        {
            _text.text = _starsManager.collectedStars + " / " + _starsManager.neededStars;
        }
    }
}