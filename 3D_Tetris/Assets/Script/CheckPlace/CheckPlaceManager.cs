using System;
using System.Collections.Generic;
using IntegerExtension;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.CheckPlace
{
    public class CheckPlaceManager: MonoBehaviour
    {
        private PlaneMatrix _matrix;
        private HeightHandler _heightHandler;
        private ElementData _elementData;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _heightHandler = RealizationBox.Instance.haightHandler;
            _elementData = ElementData.Instance;
            Invoke(nameof(LastStart), 1f);
        }

        private void LastStart()
        {
            _elementData.onNewElementUpdate += CheckCurrentPlace;
            RealizationBox.Instance.FSM.AddListener( TetrisState.Move, CheckCurrentPlace);
        }
        
        public void CheckCurrentPlace()
        {
            if (_elementData.newElement == null)
                return ;

            if (RealizationBox.Instance.bombsManager.isBombFalling)
                return;

            HighlightLoseGame(_elementData.newElement, !CheckPlace(_elementData.newElement, Vector3Int.zero));
        }

        public bool CheckAllPosition(Element element)
        {
            foreach (var direction in (move[]) Enum.GetValues(typeof(move)))
            {
                var vectorDirection = SetVectorMove(direction);

                for (int i = 1; i <= 2; i++)
                {
                    if (_matrix.CheckEmptyPlaсe(element, vectorDirection * i, true))
                    {
                        if (CheckPlace(_elementData.newElement, vectorDirection * i))
                            return true;

                    }
                }
            }
            
            return false;
        }
        
        private Vector3Int SetVectorMove(move direction)
        {
            Vector3Int vectorDirection;
            if (direction == move.x)
                vectorDirection = new Vector3Int(1, 0, 0);
            else if (direction == move.xm)
                vectorDirection = new Vector3Int(-1, 0, 0);
            else if (direction == move.z)
                vectorDirection = new Vector3Int(0, 0, 1);
            else // (direction == move._z)
                vectorDirection = new Vector3Int(0, 0, -1);

            return vectorDirection;
        }
        
        private bool CheckPlace(Element element, Vector3Int offset)
        {
            var allXZ = element.projectionBlocks;
            
            // find max different 
            int diffY = 100;
            foreach (var b in allXZ)
            {
                int x = b.coordinates.x + offset.x;
                int z = b.coordinates.z + offset.z;
                
                var _minY = _matrix.MinHeightInCoordinates(x.ToIndex(), z.ToIndex());
                            //_matrix.MinHeightInCoordinates(b.coordinates.x.ToIndex(), b.coordinates.z.ToIndex());

                int currentDiff = b._coordinates.y - _minY;
                if (currentDiff < diffY)
                    diffY = currentDiff;
            }

            // check out of limit
            foreach (var b in element.blocks)
            {
                if (b.coordinates.y - diffY >= _heightHandler.limitHeight)
                    return false;
            }

            return true;
        }

        private void HighlightLoseGame(Element element, bool isHighlight)
        {
            foreach (var b in element.blocks)
            {
               b.Outline(isHighlight);
            }
        }
    }
}