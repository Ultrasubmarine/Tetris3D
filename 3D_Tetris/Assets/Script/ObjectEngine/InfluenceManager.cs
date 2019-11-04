using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.ObjectEngine
{
    public class InfluenceManager : MonoBehaviour
    {
        private List<IInfluence> _influences;

        [SerializeField] private Transform testObj;

        private void Awake()
        {
            _influences = new List<IInfluence>();
        }

        public void AddFakeMove()
        {
            AddDrop(testObj, Vector3.right, 1);
        }

        public void AddDrop(Transform obj, Vector3 offset, float speed, Action callBack = null)
        {
            var info = new DropInfluence(obj,offset, speed, callBack);
            _influences.Add(info);
        }
        
        public void AddMove(Element element, Vector3 offset, float speed, Action callBack = null)
        {
            var info = new MoveInfluence(element,offset, speed, callBack);
            _influences.Add(info);
        }

        private void Update()
        {
            var i = 0;
            while (i < _influences.Count)
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