﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;

public class Projection : MonoBehaviour
{
    private TetrisFSM _fsm;
    private PlaneMatrix _matrix;

    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _HeightProjection = 0.1f;

    private List<GameObject> _projectionsList = new List<GameObject>();
    private Pool<GameObject> _pool;
    
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _fsm = RealizationBox.Instance.FSM;

        _pool = new Pool<GameObject>(_prefab);
        Invoke(nameof(LastStart), 1f);
    }

    private void LastStart()
    {
        ElementData.NewElementUpdate += CreateProjection;

        _fsm.AddListener(TetrisState.Move, CreateProjection);
        _fsm.AddListener(TetrisState.MergeElement, () => Destroy());
    }

    private void CreateProjection()
    {
        var obj = ElementData.NewElement;

        Destroy();

        var positionProjection = obj.MyBlocks.Select(b => b.XZ).Distinct();

        foreach (var item in positionProjection)
        {
            float y = _matrix.MinHeightInCoordinates(item.x.ToIndex(), item.z.ToIndex());

            var posProjection = new Vector3(item.x, y + _HeightProjection, item.z);

            var o = _pool.Pop(true);
            o.transform.position = posProjection;
            _projectionsList.Add(o);
        }
    }

    private void Destroy()
    {
        foreach (var item in _projectionsList) _pool.Push(item);
        _projectionsList.Clear();
    }
}