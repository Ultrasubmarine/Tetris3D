using System;
using System.Collections.Generic;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.Projections
{
    public struct ProjectionLineStruct
    {
        public ProjectionLine projection;
        public Block block;

        public ProjectionLineStruct(ProjectionLine projection, Block block)
        {
            this.projection = projection;
            this.block = block;
        }
    }
    
    public class ProjectionLineManager : MonoBehaviour
    {
        [SerializeField] private ProjectionLine _prefab;

        [SerializeField] private Transform _projectionObject;
        
        private List<ProjectionLineStruct> _projections;
        private Pool<ProjectionLine> _pool;
        private PlaneMatrix _matrix;
        private float _lateYElementPosition;
        
        private void Start()
        {
            _projections = new List<ProjectionLineStruct>();
            _pool = new Pool<ProjectionLine>(_prefab, this.transform);
            _matrix = RealizationBox.Instance.matrix;

            ElementData.onNewElementUpdate += UpdateProjectionLines;
        }

        public void UpdateProjectionLines()
        {
            Clear();

             foreach (var block in ElementData.newElement.projectionBlocks)
             {
                 var o = _pool.Pop(true);
                o.transform.localPosition = new Vector3(block.xz.x, 0, block.xz.z);
                
                float minHeightInCoordinates = _matrix.MinHeightInCoordinates(block.xz.x.ToIndex(), block.xz.z.ToIndex());
                o.SetMinBottomHeight(minHeightInCoordinates);

                o.SetHeight(block._coordinates.y);
                _projections.Add(new ProjectionLineStruct(o, block));
             }

             _lateYElementPosition = ElementData.newElement.transform.position.y;
        }

        private void Update()
        {
            if (ElementData.newElement == null)
                return;
            
            var delta = _lateYElementPosition - ElementData.newElement.transform.position.y;
            foreach (var proj in _projections)
            {
                proj.projection.IncrementHeight( delta );
            }
            _lateYElementPosition = ElementData.newElement.transform.position.y;
        }
        
        public void Clear()
        {
            foreach (var projectionLine in _projections)
            {
                _pool.Push(projectionLine.projection);
            }
            _projections.Clear();
        }

        public void OnDestroy()
        {
            ElementData.onNewElementUpdate -= UpdateProjectionLines;
        }
    }
}