using System;
using System.Linq;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementCleaner : MonoBehaviour
    {
        public event Action onDeleteAllElements;
        
        private PlaneMatrix _matrix;
        private GameLogicPool _pool;

        private Transform _myTransform;

        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _pool = RealizationBox.Instance.gameLogicPool;

            _myTransform = transform;
        }

        public void CutElement()
        {
            var k = 0;
            var countK = ElementData.mergerElements.Count;
            while (k < countK)
            {
                var cutBlocks = ElementData.mergerElements[k].GetNotAttachedBlocks();
                if (cutBlocks != null)
                {
                    var newElement = _pool.CreateEmptyElement();
                    newElement.name = ElementData.mergerElements[k] + "1" ;
                    newElement.myTransform.localPosition = ElementData.mergerElements[k].myTransform.localPosition;
                    newElement.SetBlocks(cutBlocks);
                    foreach (var block in newElement.blocks) block.myTransform.parent = newElement.myTransform;

                    newElement._isBind = true;

                    ElementData.mergerElements.Add(newElement);
                    newElement.myTransform.parent = _myTransform;
                    countK++;
                }
                
                k++;
            }
        }

        public void ClearElementsFromDeletedBlocks()
        {
            foreach (var element in ElementData.mergerElements)
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
            foreach (var item in deletedList) _pool.DeleteBlock(item);
        }

        private void DeleteEmptyElement()
        {
            var elements = ElementData.mergerElements;
            for (var i = 0; i < elements.Count;)
                if (elements[i].CheckEmpty())
                {
                    var tmp = elements[i];
                    ElementData.mergerElements.Remove(elements[i]);
                    _pool.DeleteElement(tmp);
                }
                else
                {
                    i++;
                }
        }

        public void DeleteAllElements()
        {
            var elements = ElementData.mergerElements;
            foreach (var item in elements)
            {
                _matrix.UnbindToMatrix(item);
                ClearDeletedBlocks(item.blocks.ToArray());
                item.RemoveBlocksInList(item.blocks.ToArray());
                _pool.DeleteElement(item);
            }

            ElementData.RemoveAll();

            if (!Equals(ElementData.newElement, null))
            {
                ClearDeletedBlocks(ElementData.newElement.blocks.ToArray());
                ElementData.newElement.RemoveBlocksInList(ElementData.newElement.blocks.ToArray());
                _pool.DeleteElement(ElementData.newElement);
//                ElementData.NewElement = null;
            }
            
            onDeleteAllElements?.Invoke();
        }
    }
}