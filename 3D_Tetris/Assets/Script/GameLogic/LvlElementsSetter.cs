using System;
using System.Collections.Generic;
using System.Linq;
using Script.GameLogic.StoneBlock;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.GameLogic
{
    [Serializable]
    public struct CreatedElement
    {
        public Material material;
        public List<Vector3Int> blocks;
        public bool isStone;
        public bool isEvilBox;
    }
    public class LvlElementsSetter: MonoBehaviour
    {
      // public bool isCreateStarElements { get; set; }
       public List<CreatedElement> createdElements;

       private PlaneMatrix _matrix;
       private GameLogicPool _pool;
       private ElementDropper _elementDropper;
       private StoneBlockManager _stoneBlockManager;
       private EvilBoxManager _evilBoxManager;
       
       public void Init()
       {
           _matrix = RealizationBox.Instance.matrix;
           _pool = RealizationBox.Instance.gameLogicPool;
           _elementDropper = RealizationBox.Instance.elementDropper;
           _stoneBlockManager = RealizationBox.Instance.stoneBlockManager;
           _evilBoxManager = RealizationBox.Instance.evilBoxManager;
       }

       private void Start()
       {
           Debug.Log("str");
       }

       public void Load(List<CreatedElement> elements)
       {
           createdElements = elements;
       }

       public void CreateElements()
       {
           var parentTransform  = RealizationBox.Instance.elementDropper.transform;
           
           foreach (var e in createdElements)
           {
               var element = _pool.CreateEmptyElement();
               foreach (var position in e.blocks)
               {
                   _pool.CreateBlock(position, element, e.material);
               }
               
               if(e.isStone)
                   _stoneBlockManager.TransformToStone(element);
               else if(e.isEvilBox)
                   foreach (var block in element.blocks)
                   {
                       _evilBoxManager.AddEvilBox(block);
                   }
               
               // выравниваем элемент относительно координат y 
               var min_y = element.blocks.Min(s => s.coordinates.y);
               var max_y = element.blocks.Max(s => s.coordinates.y);

               var size = max_y - min_y;

               var pos = _elementDropper.transform.position;
             
              // int currentHeightPosition  = (_matrix.height - _minHeight) * _gameCamera.lastMaxCurrentHeight / _heightHandler.limitHeight + _minHeight; //(_matrix.height - _minHeight) * _heightHandler.currentHeight / _heightHandler.limitHeight + _minHeight;

             //  int currentYpos = fixedHightPosition == 0 ? currentHeightPosition : fixedHightPosition;
              // element.InitializationAfterGeneric(currentYpos);
               element.myTransform.position = new Vector3(pos.x, pos.y /*+ currentYpos - size*/, pos.z);
               element.myTransform.parent = parentTransform;
               ElementData.Instance.MergeElement(element);
               _matrix.BindToMatrix(element);
               element.isPreconstruct = true;
           }
       }
    }
}