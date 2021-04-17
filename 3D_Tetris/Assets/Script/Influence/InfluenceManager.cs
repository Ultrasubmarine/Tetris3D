using System;
using System.Collections.Generic;
using Script.ObjectEngine;
using UnityEngine;

namespace Script.Influence
{
    public class InfluenceManager : MonoBehaviour
    {
        private float currentSpeed = 1;
        public float speed { get; set; } = 1;
        
        [SerializeField] private Transform _testObj;
        
        private List<IInfluence> _influences;

        private List<IInfluence> _moveInfluences;

        private TetrisFSM _fsm;

        private SlowManager _slowler;

        [SerializeField] private int _faster = 0;

        private bool _fastMode = false;
        
        private void Awake()
        {
            _influences = new List<IInfluence>();
            _moveInfluences = new List<IInfluence>();
            
        }

        private void Start()
        {
            _fsm = RealizationBox.Instance.FSM;
            _slowler = RealizationBox.Instance.slowManager;

            _slowler.onUpdateValue += CalculateSpeed;
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
        }
        
        private void FixedUpdate()
        {
            var i = 0;
            if(_fsm.GetCurrentState() == TetrisState.Move)
            {
                while (i < _moveInfluences.Count)
                {
                    var item = _moveInfluences[i];

                    if (item.Update())
                        _moveInfluences.Remove(item);
                    else
                        i++;
                }
            }
            
            i = 0;
            while (i < _influences.Count)
            {
                var item = _influences[i];

                if (item.Update(currentSpeed))
                    _influences.Remove(item);
                else
                    i++;
            }
        }

        public void CalculateSpeed()
        {
            if(!_fastMode)
                currentSpeed = Mathf.Clamp(speed - _slowler.slow, 0 ,1);
            else
                currentSpeed = speed + _faster;
        }

        public void SetSpeedMode(bool mode)
        {
            if (mode)
            {
                _fastMode = true;
                currentSpeed = speed + _faster;
            }
            else
            {
                _fastMode = false;
                CalculateSpeed();
            }
        }
    }
}