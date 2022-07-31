using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Helper.Patterns;
using IntegerExtension;
using JetBrains.Annotations;
using Script.CheckPlace;
using Script.GameLogic.TetrisElement;

public class Projection : MonoBehaviour
{
    private TetrisFSM _fsm;
    private PlaneMatrix _matrix;
    private ElementData _elementData;
    private CheckPlaceManager _checkPlaceManager;
    
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _heightProjection = 0.1f;

    private List<GameObject> _projectionsList = new List<GameObject>();
    private Pool<GameObject> _pool;

    [SerializeField] private Material _noEmptyPlaceMaterial;
    [SerializeField] private Material _emptyPlaceMaterial;
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _fsm = RealizationBox.Instance.FSM;
        _elementData = ElementData.Instance;
        _checkPlaceManager = RealizationBox.Instance.checkPlaceManager;
        
        _pool = new Pool<GameObject>(_prefab, transform);
        Invoke(nameof(LastStart), 1f);
    }
    
    private void LastStart()
    {
        _elementData.onNewElementUpdate += CreateProjection;
        
        _fsm.AddListener(TetrisState.MergeElement, () => Destroy());
    }

    public void CreateProjection()
    {
        var diffY = _checkPlaceManager.currentDiffY;
        var obj = _elementData.newElement;

        Destroy();

     //  List<CoordinatXZ> used = new List<CoordinatXZ>();

       List<Block> usedBlock = new List<Block>();
       foreach (var b in obj.blocks)
       {
           var index = usedBlock.FindIndex(newB => newB.xz == b.xz);

           if (index > -1)
           {
               if (usedBlock[index]._coordinates.y > b._coordinates.y)
               {
                   usedBlock.RemoveAt(index);
                   usedBlock.Add(b);
               }
           }
           else
               usedBlock.Add(b);
       }
       
       foreach (var block in usedBlock)
        {
          //  if(used.Contains(block.xz)) // recalculate color
          //      continue;
            
            float y = _matrix.MinHeightInCoordinatesAfterPoint(block.xz.x.ToIndex(), block.xz.z.ToIndex(), block.coordinates.y);

            var posProjection = new Vector3(block.xz.x, y + _heightProjection, block.xz.z);

            var o = _pool.Pop(true);
            o.transform.localPosition = posProjection;
            _projectionsList.Add(o);

            if (diffY != -1)
            {
                if (block.coordinates.y - diffY > y)
                {
                    o.GetComponent<MeshRenderer>().material = _emptyPlaceMaterial;
                    continue;
                }
            }
            o.GetComponent<MeshRenderer>().material = _noEmptyPlaceMaterial;
         //   used.Add(block.xz);
        }
    }
    
    public void Destroy()
    {
        foreach (var item in _projectionsList) _pool.Push(item);
        _projectionsList.Clear();
    }

    private void OnDestroy()
    {
        _elementData.onNewElementUpdate -= CreateProjection;
    }
}