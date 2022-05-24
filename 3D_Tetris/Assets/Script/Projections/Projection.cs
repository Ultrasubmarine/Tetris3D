using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Helper.Patterns;
using IntegerExtension;
using JetBrains.Annotations;
using Script.GameLogic.TetrisElement;

public class Projection : MonoBehaviour
{
    private TetrisFSM _fsm;
    private PlaneMatrix _matrix;
    private ElementData _elementData;
    
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _heightProjection = 0.1f;

    private List<GameObject> _projectionsList = new List<GameObject>();
    private Pool<GameObject> _pool;
    
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _fsm = RealizationBox.Instance.FSM;
        _elementData = ElementData.Instance;
        
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
        var obj = _elementData.newElement;

        Destroy();

       List<CoordinatXZ> used = new List<CoordinatXZ>();
       foreach (var block in obj.blocks)
        {
            if(used.Contains(block.xz))
                continue;
            
            float y = _matrix.MinHeightInCoordinatesAfterPoint(block.xz.x.ToIndex(), block.xz.z.ToIndex(), block.coordinates.y);

            var posProjection = new Vector3(block.xz.x, y + _heightProjection, block.xz.z);

            var o = _pool.Pop(true);
            o.transform.localPosition = posProjection;
            _projectionsList.Add(o);
            
            used.Add(block.xz);
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