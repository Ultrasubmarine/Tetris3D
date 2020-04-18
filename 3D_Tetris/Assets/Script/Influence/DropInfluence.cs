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
        
        public DropInfluence(Transform transform, Vector3 direction, float allTime, Action action)
        {
            _transform = transform;
            
            _start = transform.position;
            _finish = transform.position + direction;
            
            _allTime = allTime;
            _currentTime = 0;
            _callBack = action;
        }
        
        public bool Update(float speed = 1)
        {
           // Debug.Log("drop update");
            if (Drop(speed))
            {
                _callBack?.Invoke();
                return true;
            }
            return false;
        }

        private bool Drop(float speed = 1)
        {
            _currentTime += Time.deltaTime * speed;
            _transform.position = Vector3.Lerp(_start, _finish, _currentTime / _allTime);

            if (_currentTime >= _allTime)
            {
                _transform.position = _finish;
                return true;
            }

            return false;
        }
    }
}