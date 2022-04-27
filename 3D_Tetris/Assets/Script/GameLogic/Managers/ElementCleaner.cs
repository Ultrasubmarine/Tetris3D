using System;
using System.Linq;
using Script.GameLogic.GameItems;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementCleaner : MonoBehaviour
    {
        public event Action onDeleteAllElements;
        
        private PlaneMatrix _matrix;
        private GameLogicPool _pool;
        private ElementData _elementData;
        
        private Transform _myTransform;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _pool = RealizationBox.Instance.gameLogicPool;
            _elementData = ElementData.Instance;
            
            _myTransform = transform;
        }

        public void CutElement()
        {
            var k = 0;
            var countK =  _elementData.mergerElements.Count;
            while (k < countK)
            {
                var cutBlocks =  _elementData.mergerElements[k].GetNotAttachedBlocks();
                if (cutBlocks != null)
                {
                    var newElement = _pool.CreateEmptyElement();
                    newElement.name =  _elementData.mergerElements[k] + "1" ;
                    newElement.myTransform.localPosition =  _elementData.mergerElements[k].myTransform.localPosition;
                    newElement.SetBlocks(cutBlocks);
                    foreach (var block in newElement.blocks) block.myTransform.parent = newElement.myTransform;

                    newElement._isBind = true;

                    _elementData.mergerElements.Add(newElement);
                    newElement.myTransform.parent = _myTransform;
                    countK++;
                }
                
                k++;
            }
        }

        public void ClearElementsFromDeletedBlocks()
        {
            foreach (var element in  _elementData.mergerElements)
            {
                var deletedList = element.blocks.Where(s => s.isDestroy).ToArray();
                if (deletedList.ToArray().Length > 0)
                {
                    element.RemoveBlocksInList(deletedList);
                    ClearDeletedBlocks(deletedList);
                }
            }

            DeleteEmptyElement();
        }

        private void ClearDeletedBlocks(Block[] deletedList)
        {
            foreach (var item in deletedList) if(!Equals(item, null))_pool.DeleteBlock(item);
        }

        private void DeleteEmptyElement()
        {
            var elements =  _elementData.mergerElements;
            for (var i = 0; i < elements.Count;)
                if (elements[i].CheckEmpty())
                {
                    var tmp = elements[i];
                    _elementData.mergerElements.Remove(elements[i]);
                    _pool.DeleteElement(tmp);
                }
                else
                {
                    i++;
                }
        }

        public void DeleteAllElements()
        {
            var elements =  _elementData.mergerElements;
            foreach (var item in elements)
            {
                _matrix.UnbindToMatrix(item);
                ClearDeletedBlocks(item.blocks.ToArray());
                item.RemoveBlocksInList(item.blocks.ToArray());
                _pool.DeleteElement(item);
            }
            
            if (!Equals( _elementData.newElement, null))
            {
                ClearDeletedBlocks( _elementData.newElement.blocks.ToArray());
                _elementData.newElement.RemoveBlocksInList( _elementData.newElement.blocks.ToArray());
                _pool.DeleteElement( _elementData.newElement);
            }

            ElementData.Instance.RemoveAll();

            onDeleteAllElements?.Invoke();
        }

        public void DeletePickableBlock(PickableBlock pBlock)
        {
            _pool.DeletePickableBlock(pBlock);
        }
        
    }
}