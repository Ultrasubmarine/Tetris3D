using System.Linq;
using Boo.Lang;
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

        public void ClearElementsAfterDeletedBlocks() 
        {
            foreach (var element in ElementData.MergerElement) 
            {
                var deletedList = element.MyBlocks.Where(s => s.IsDestroy).ToArray();
                if (deletedList.ToArray().Length > 0) {               
                    element.DeleteBlocksInList(deletedList);
                    ClearDeleteBlocks(deletedList);
                }
            }
            DeleteEmptyElement();
        }
    
        private void ClearDeleteBlocks(Block[] deletedList) {
            foreach (var item in deletedList) {
                _generator.DeleteBlock(item);
            }        
        }
        
        private void DeleteEmptyElement() {       
            for (int i = 0; i < ElementData.MergerElement.Count;) {
                if (ElementData.MergerElement[i].CheckEmpty())
                {
                    Element tmp = ElementData.MergerElement[i];
                    ElementData.MergerElement.Remove(ElementData.MergerElement[i]);
                   
                    _generator.DeleteElement(tmp); 
                }
                else {
                    i++;
                }
            }
        }

        void DeleteAllElements() {
            while (ElementData.MergerElement.Count > 0) {
                Element tmp = ElementData.MergerElement[0];
                
                _matrix.UnbindToMatrix(tmp);
                ElementData.MergerElement.Remove(tmp);
                
                ClearDeleteBlocks( tmp.MyBlocks.ToArray() );
                tmp.DeleteBlocksInList( tmp.MyBlocks.ToArray() );
                _generator.DeleteElement(tmp);
            }
            if (!Equals(ElementData.NewElement, null)) {
                
                ClearDeleteBlocks(ElementData.NewElement.MyBlocks.ToArray());
                ElementData.NewElement.DeleteBlocksInList( ElementData.NewElement.MyBlocks.ToArray() );
                _generator.DeleteElement(ElementData.NewElement);
                ElementData.NewElement = null;
            }
        }

        public void CutElement() {

            int k = 0;
            int countK = ElementData.MergerElement.Count;
            while (k < countK) {
                System.Collections.Generic.List<Block> cutBlocks = ElementData.MergerElement[k].GetNotAttachedBlocks();
                if (cutBlocks != null) {
                    Element newElement = _generator.CreateEmptyElement();
                    newElement.MyTransform.position = ElementData.MergerElement[k].MyTransform.position;
                    newElement.MyBlocks = cutBlocks;
                    foreach (var block in newElement.MyBlocks)
                    {
                        block.MyTransform.parent = newElement.MyTransform;
                    }
                    
                    _matrix.UnbindToMatrix(newElement);
                    _matrix.UnbindToMatrix(ElementData.MergerElement[k]);

                    ElementData.MergerElement.Add(newElement);
                    newElement.MyTransform.parent = _myTransform;
                    countK++;
                }
                k++;
            }
        }
    }
}