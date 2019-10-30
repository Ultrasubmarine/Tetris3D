using System;
using UnityEngine;

namespace Script.ObjectEngine
{
    public enum InfluenceMode
    {
        Move,
        Turn,
        Drop,
    }
    
    public struct Influence
    {
        private readonly Transform _transform;
        private readonly Vector3 _start;
        private readonly Vector3 _finish;

        private readonly float _allTime;
        private float _currentTime;

        private Func<bool> _action;
        private Action _callBack;
        
        public Influence(Transform transform, Vector3 finish, float allTime, InfluenceMode mode, Action callBack = null ) : this()
        {
            _transform = transform;

            _start = transform.position;
            _finish = finish;

            _allTime = allTime;
            _currentTime = 0;

            switch (mode)
            {
                case InfluenceMode.Drop:
                {
                    _action = Drop;
                    break;
                }
                case InfluenceMode.Move:
                {
                    _action = Move;
                    break;
                }
                case InfluenceMode.Turn:
                {
                    _action = Turn;
                    break;
                }
            }

            _callBack = callBack;
        }
    
        /// <summary>
        ///   <para> Return true if move finish </para>
        /// </summary>
        public bool Update()
        {
            if( _action.Invoke()) 
            {
                _callBack?.Invoke();
                return true;
            }
            return false;
        }

        private bool Turn()
        {
            return false;
        }

        private bool Move()
        {
            _currentTime += Time.deltaTime;
            _transform.position = Vector3.Lerp(_start, _finish, _currentTime / _allTime);

            Debug.Log("Move update");
            if (_currentTime >= _allTime)
            {
                _transform.position = _finish;
                return true;
            }
            return false;
        }
        
        private bool Drop()
        {
            return false;
        }
    }
}