using System;
using System.Collections.Generic;
using Script.ObjectEngine;
using UnityEngine;

namespace Script.Influence
{
    public class InfluenceManager : MonoBehaviour
    {
        private List<IInfluence> _influences;

        private List<IInfluence> _moveInfluences;
        
        [SerializeField] private Transform testObj;

        private TetrisFSM _fsm;
        
        private void Awake()
        {
            _influences = new List<IInfluence>();
            _moveInfluences = new List<IInfluence>();
        }

        private void Start()
        {
            _fsm = RealizationBox.Instance.FSM;
        }

        public void AddDrop(Transform obj, Vector3 offset, float speed, Action callBack = null)
        {
            var info = new DropInfluence(obj,offset, speed, callBack);
            _influences.Add(info);
        }

        public void AddMove(Element element, Vector3 offset, float speed, Action callBack = null)
        {
            var info = new MoveInfluence(element,offset, speed, callBack);
            _moveInfluences.Add(info);
//            _influences.Add(info);
        }

        private void Update()
        {
            var i = 0;
            if(_fsm.GetCurrentState() == TetrisState.Move ||
               _fsm.GetCurrentState() == TetrisState.MoveMode)
            {
                while (i < _moveInfluences.Count)
                {
                    var item = _moveInfluences[i];

                    if (item.Update())
                        _moveInfluences.Remove(item);
                    else
                        i++;
                }
                return;
            }
            
            i = 0;
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