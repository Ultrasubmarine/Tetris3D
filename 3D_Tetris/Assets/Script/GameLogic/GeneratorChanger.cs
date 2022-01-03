using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameLogic
{
    [Serializable]
    struct ChangeGeneratorInfo
    {
        public float score;
        
        [Tooltip(" v kakie storoni rastet")] 
        public int stepOfHardElement; // 1-min 3-max
        [Tooltip(" generate block in other side")] 
        public bool growBlocksAnywhere; // grow more hard element
        
        public ChangeGeneratorInfo(float score, int stepOfHardElement, bool growBlocksAnywhere)
        {
            this.score = score;
            this.stepOfHardElement = stepOfHardElement;
            this.growBlocksAnywhere = growBlocksAnywhere;
        }
    }
    
    public class GeneratorChanger : MonoBehaviour
    {
    
        [SerializeField] private List<ChangeGeneratorInfo> _points;
    
        private Score _score;

        private int _currentIndex = 0;

        private ChangeGeneratorInfo _startSettings;

        private Generator _generator;
    
        public void ResetGenerator()
        {
            _currentIndex = 0;
            SetGeneratorSettings(_startSettings);
        }
    
        private void Start()
        {
            _score = RealizationBox.Instance.score;
            _score.onScoreIncrement += onIncrementScore;

            _generator = RealizationBox.Instance.generator;
            _startSettings = new ChangeGeneratorInfo(0, _generator.stepOfHardElement, _generator.growBlocksAnywhere);
        }

        private void onIncrementScore(int increment)
        {
            if (_currentIndex == _points.Count)
                return;
            
            if (_score.currentScore >= _points[_currentIndex].score)
            {
                SetGeneratorSettings(_points[_currentIndex++]);
                Debug.Log("update generator settings");
            }
        }

        private void SetGeneratorSettings(ChangeGeneratorInfo newSettings)
        {
            _generator.stepOfHardElement = newSettings.stepOfHardElement;
            _generator.growBlocksAnywhere = newSettings.growBlocksAnywhere;
        }
    }
}