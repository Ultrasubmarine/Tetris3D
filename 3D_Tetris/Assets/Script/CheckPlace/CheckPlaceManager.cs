using System;
using System.Collections.Generic;
using IntegerExtension;
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

            HighlightLoseGame(_elementData.newElement, !CheckPlace(_elementData.newElement));
        }

        private bool CheckPlace(Element element)
        {
            var allXZ = element.projectionBlocks;
            
            // find max different 
            int diffY = 100;
            foreach (var b in allXZ)
            {
                var _minY = _matrix.MinHeightInCoordinates(b.coordinates.x.ToIndex(), b.coordinates.z.ToIndex());

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