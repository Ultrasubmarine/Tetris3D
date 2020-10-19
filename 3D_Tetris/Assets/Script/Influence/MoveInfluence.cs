using System;
using UnityEngine;

namespace Script.Influence
{
    public struct MoveInfluence : IInfluence
    {
        private readonly Element element;
        
        private readonly Vector3 _start;
        
        private readonly Vector3 _finish;
        
        private Vector3 _lastDeltaVector;
        
        private Vector3[] _finalPosBlock;
        
        private readonly float _allTime;
        
        private float _currentTime;

        private Action _callBack;
        
        public MoveInfluence(Element element, Vector3 direction, float allTime, Action action)
        {
            this.element = element;

            _start = Vector3.zero;
            _finish = direction;
            _lastDeltaVector = Vector3.zero;
            
            _finalPosBlock = new Vector3[element.blocks.Count];

            for (var i = 0; i < element.blocks.Count; i++) 
                _finalPosBlock[i] = element.blocks[i].myTransform.localPosition + direction;
            
            _allTime = allTime;
            _currentTime = 0;
            _callBack = action;
        }

        public bool Update(float speed = 1)
        {
            if (Move())
            {
                _callBack?.Invoke();
                return true;
            }
            return false;
        }
        
        private bool Move(float speed = 1)
        {
            if (_currentTime + Time.deltaTime < _allTime)
            {
                _currentTime += Time.deltaTime * speed;
                
                var deltaVector = Vector3.Lerp(_start, _finish, _currentTime / _allTime);

                foreach (var block in element.blocks)
                    block.myTransform.localPosition += deltaVector - _lastDeltaVector;
                _lastDeltaVector = deltaVector;
                return false;
            }

            for (var i = 0; i < element.blocks.Count; i++)
                element.blocks[i].myTransform.localPosition =
                    new Vector3(_finalPosBlock[i].x, element.blocks[i].transform.localPosition.y, _finalPosBlock[i].z);
            return true;
        }

        public static void MomentaryMove(Element element, Vector3 direction)
        {
            Vector3[] _finalPosBlock = new Vector3[element.blocks.Count];

            for (var i = 0; i < element.blocks.Count; i++) 
                _finalPosBlock[i] = element.blocks[i].myTransform.localPosition + direction;
            
            for (var i = 0; i < element.blocks.Count; i++)
                element.blocks[i].myTransform.localPosition =
                    new Vector3(_finalPosBlock[i].x, element.blocks[i].transform.localPosition.y, _finalPosBlock[i].z);
        }
    }
}