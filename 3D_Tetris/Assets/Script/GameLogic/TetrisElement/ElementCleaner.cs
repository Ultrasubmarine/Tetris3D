using System.Linq;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementCleaner : MonoBehaviour
    {
        private PlaneMatrix _matrix;
        private Generator _generator;
        
        private Transform _myTransform;
        
        private void Start()
        {
            _matrix = RealizationBox.Instance.Matrix();
            _generator = RealizationBox.Instance.ElementGenerator();

            _myTransform = transform;
        }

        public void CutElement() {

            int k = 0;
            int countK = ElementData.MergerElements.Count;
            while (k < countK) {
                System.Collections.Generic.List<Block> cutBlocks = ElementData.MergerElements[k].GetNotAttachedBlocks();
                if (cutBlocks != null) {
                    Element newElement = _generator.CreateEmptyElement();
                    newElement.MyTransform.position = ElementData.MergerElements[k].MyTransform.position;
                    newElement.MyBlocks = cutBlocks;
                    foreach (var block in newElement.MyBlocks)
                    {
                        block.MyTransform.parent = newElement.MyTransform;
                    }
                    
                    _matrix.UnbindToMatrix(newElement);
                    _matrix.UnbindToMatrix(ElementData.MergerElements[k]);

                    ElementData.MergerElements.Add(newElement);
                    newElement.MyTransform.parent = _myTransform;
                    countK++;
                }
                k++;
            }
        }
        
        public void ClearElementsFromDeletedBlocks() 
        {
            foreach (var element in ElementData.MergerElements) 
            {
                var deletedList = element.MyBlocks.Where(s => s.IsDestroy).ToArray();
                if (deletedList.ToArray().Length > 0) {               
                    element.RemoveBlocksInList(deletedList);
                    ClearDeletedBlocks(deletedList);
                }
            }
            DeleteEmptyElement();
        }
        
        private void ClearDeletedBlocks(Block[] deletedList) {
            foreach (var item in deletedList) {
                _generator.DeleteBlock(item);
            }        
        }
        
        private void DeleteEmptyElement() 
        {
            var elements = ElementData.MergerElements;
            for (int i = 0; i < elements.Count;) {
                if (elements[i].CheckEmpty())
                {
                    var tmp = elements[i];
                    ElementData.MergerElements.Remove(elements[i]);
                    _generator.DeleteElement(tmp); 
                }
                else
                    i++;
            }
        }

        void DeleteAllElements()
        {
            var elements = ElementData.MergerElements;
            foreach (var item in elements)
            {
                _matrix.UnbindToMatrix(item);
                ClearDeletedBlocks( item.MyBlocks.ToArray() );
                item.RemoveBlocksInList( item.MyBlocks.ToArray() );
                _generator.DeleteElement(item);
            }
            
            while (ElementData.MergerElements.Count > 0) {
                Element tmp = ElementData.MergerElements[0];
                
                _matrix.UnbindToMatrix(tmp);
                ElementData.MergerElements.Remove(tmp);
                
                ClearDeletedBlocks( tmp.MyBlocks.ToArray() );
                tmp.RemoveBlocksInList( tmp.MyBlocks.ToArray() );
                _generator.DeleteElement(tmp);
            }
            if (!Equals(ElementData.NewElement, null)) {
                
                ClearDeletedBlocks(ElementData.NewElement.MyBlocks.ToArray());
                ElementData.NewElement.RemoveBlocksInList( ElementData.NewElement.MyBlocks.ToArray() );
                _generator.DeleteElement(ElementData.NewElement);
//                ElementData.NewElement = null;
            }
        }
    }
}