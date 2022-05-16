using System;
using UnityEngine;

namespace Script.GameLogic
{
    public class NextElementUI : MonoBehaviour
    {
        [SerializeField] private Transform _nextElementParent;
        
        private Element _nextElement;
        private GameLogicPool _pool;
        
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            
            _nextElement= _pool.CreateEmptyElement();
            _nextElement.myTransform.parent = _nextElementParent;
            _nextElement.myTransform.localPosition = Vector3.zero;
            _nextElement.myTransform.localRotation = Quaternion.identity;
            _nextElement.myTransform.localScale = Vector3.one * 70;

            RealizationBox.Instance.islandTurn.extraTurn.Add(_nextElement.myTransform);
         //   RealizationBox.Instance.
        }

        private void CreateNextElement( Element element, Material material)
        {
            float xMax, zMax , yMax, xMin, zMin, yMin;
            xMax = zMax = yMax = int.MinValue;
            xMin = zMin = yMin = int.MaxValue;
        
            for(int i = 0; i<element.blocks.Count; i++)
            {
                var position = element.blocks[i]._coordinates;
                Vector3Int v = new Vector3Int((int) position.x, (int) position.y, (int) position.z);
                _pool.CreateBlock(v, _nextElement,material);

                Vector3 ansPos = _nextElement.blocks[i].myTransform.localPosition;
                xMax = xMax < ansPos.x? ansPos.x : xMax;
                zMax = zMax < ansPos.z? ansPos.z : zMax;
                yMax = yMax < ansPos.y? ansPos.y : yMax;
            
                xMin = xMin > ansPos.x? ansPos.x : xMin;
                zMin = zMin > ansPos.z? ansPos.z : zMin;
                yMin = yMin > ansPos.y? ansPos.y : yMin;
            }

            float xCenter, zCenter, yCenter;
            xCenter = (xMax + xMin) / 2f;
            zCenter = (zMax + zMin) / 2f;
            yCenter = (yMax + yMin) / 2f;
        
            foreach (var block in _nextElement.blocks)
            {
                Vector3 np = block.myTransform.localPosition - new Vector3(xCenter, yCenter, zCenter);
                block.myTransform.localPosition = np;
                block.myTransform.localScale = Vector3.one * 0.97f;
            }
        }
    }
}