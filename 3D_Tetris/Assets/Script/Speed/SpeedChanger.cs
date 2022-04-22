using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Speed
{
    [Serializable]
    public struct ChangeSpeedInfo
    {
        public float time;
        public float score;
    }
    
    public class SpeedChanger : MonoBehaviour
    {
        [SerializeField] private List<ChangeSpeedInfo> _points;
        
        private Score _score;

        private int currentIndex = 0;

        private float _startTimeDrop;
        
        
        public void ResetSpeed()
        {
            currentIndex = 0;
            global::Speed.SetTimeDrop( _startTimeDrop);
        }

        public void SetSpeedPoints(List<ChangeSpeedInfo> points)
        {
            _points = new List<ChangeSpeedInfo>();
            _points = points;
            if (_points.Count > 0 && _points[0].score == 0)
            {
                global::Speed.SetTimeDrop(_points[0].time);
            }
        }
        
        private void Start()
        {
            _score = RealizationBox.Instance.score;
            _score.onScoreIncrement += onIncrementScore;
            
            _startTimeDrop = global::Speed.timeDrop;
        }

        private void onIncrementScore(int increment)
        {
            if (currentIndex == _points.Count)
                return;
            if (_score.currentScore >= _points[currentIndex].score)
            {
                
                global::Speed.SetTimeDrop(_points[currentIndex].time);
                //Debug.Log("speed = " +  global::Speed.timeDrop);
                currentIndex++;
            }
        }
    }
}