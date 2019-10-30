using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.ObjectEngine
{
    public class InfluenceManager : MonoBehaviour
    {
        private List<Influence> _influences;

        [SerializeField] private Transform testObj;

        private void Awake()
        {
            _influences = new List<Influence>();
        }

        public void AddFakeMove()
        {
            AddMove(testObj, Vector3.right, 1);
        }
    
        public void AddMove(Transform obj, Vector3 offset, float speed, Action callBack = null)
        {
            var info = new Influence(obj, obj.position + offset, speed, InfluenceMode.Move, callBack);
            _influences.Add( info);
        }

        private void Update()
        {
            int i = 0;
            while(i < _influences.Count)
            {
                var item = _influences[i];
            
                if (item.Update())
                    _influences.Remove(item);
                else 
                    i++;
            }
        }
    }
}