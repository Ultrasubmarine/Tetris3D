using System;
using UnityEngine;

namespace Script.Influence
{
    public enum InfluenceMode
    {
        Move,
        Turn,
        Drop,
    }

    public interface IInfluence
    {
        bool Update();
    }

    public struct Influence<T>
    {
        private Func<bool> _action;
        private Action _callBack;

        public Influence(Transform transform, T finish, float allTime, Action callBack = null): this()
        {
            _action = null;
            _callBack = callBack;
        }

        /// <summary>
        ///   <para> Return true if move finish </para>
        /// </summary>
        public bool Update()
        {
            if (_action.Invoke())
            {
                _callBack?.Invoke();
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