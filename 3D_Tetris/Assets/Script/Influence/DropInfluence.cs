using System;
using Script.Influence;
using UnityEngine;

namespace Script.ObjectEngine
{
    public struct DropInfluence  : IInfluence
    {
        private readonly Transform _transform;
        private readonly Vector3 _start;
        private readonly Vector3 _finish;

        private readonly float _allTime;
        private float _currentTime;

        private Action _callBack;
        private bool _isIgnoreSlow;

        private float _deltaForCheckNear; // max delta for nearPosition == true
        private bool _isNear;
        
        public DropInfluence(Transform transform, Vector3 direction, float allTime, Action action,  float deltaForCheckNear, bool isIgnoreSlow = false)
        {
            _transform = transform;
        
            _start = transform.localPosition;
            _finish = transform.localPosition + direction;
            
            _allTime = allTime;
            _currentTime = 0;
            _callBack = action;
            _isIgnoreSlow = isIgnoreSlow;

            _deltaForCheckNear = deltaForCheckNear;
            _isNear = false;
        }
        
        public bool Update(float speed = 1)
        {
            if (Drop(speed))
            {
               if( IsNearStartPosition() )
                   Debug.Log("MOVE WINDOW");
         
               
                _callBack?.Invoke();
                return true;
            }
            return false;
        }

        public void UnlinkCallback()
        {
            _callBack = null;
        }

        public bool IsIgnoreSlow()
        {
            return _isIgnoreSlow;
        }

        public bool IsNearStartPosition() // delay for move without checking upper blocks
        {
            return _isNear;
        }

        private bool Drop(float speed = 1)
        {
            _currentTime += Time.fixedDeltaTime /*(Time.deltaTime < 0.05? Time.deltaTime : 0.05f)*/ * speed;
            _transform.localPosition = Vector3.Lerp(_start, _finish, _currentTime / _allTime);
            
           _isNear = Math.Abs(_transform.localPosition.y - _finish.y) < _deltaForCheckNear;

           if (_currentTime >= _allTime)
            {
                _transform.localPosition = _finish;
                return true;
            }

            return false;
        }
    }
}