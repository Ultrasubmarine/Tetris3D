using System;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class BestScoreText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private void Start()
        {
            _text.text = PlayerSaveProfile.instance._bestScore.ToString();
        }
    }
}