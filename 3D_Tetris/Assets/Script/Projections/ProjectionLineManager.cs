using System;
using System.Collections.Generic;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.GameItems;
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
        private Dictionary<PickableBlock, ProjectionLineStruct> _pickableProjections;
        
        private Pool<ProjectionLine> _pool;
        private PlaneMatrix _matrix;
        private float _lateYElementPosition;
        
        private void Start()
        {
            _projections = new List<ProjectionLineStruct>();
            _pickableProjections = new Dictionary<PickableBlock, ProjectionLineStruct>();
            
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
        
        public void AddPickableProjection(PickableBlock pBlock)
        {
            var o = _pool.Pop(true);
            o.transform.localPosition = new Vector3(pBlock.xz.x, 0, pBlock.xz.z);
            
            float minHeightInCoordinates = _matrix.MinHeightInCoordinates(pBlock.xz.x.ToIndex(), pBlock.xz.z.ToIndex());
            o.SetMinBottomHeight(minHeightInCoordinates);

            o.SetHeight(pBlock._coordinates.y);
            _pickableProjections.Add(pBlock, new ProjectionLineStruct(o, pBlock));
            
            o.SetLineColor(new Color(1,1,1,0.3f), true);
            pBlock.onPick += DeletePickableProjection;
        }

        public void UpdatePickableProjections()
        {
            foreach (var proj in _pickableProjections.Values)
            {
                var blockXZ = proj.block.xz;
                float minHeightInCoordinates = _matrix.MinHeightInCoordinates(blockXZ.x.ToIndex(), blockXZ.z.ToIndex());
                proj.projection.SetMinBottomHeight(minHeightInCoordinates);
            }
        }
        
        public void DeletePickableProjection(PickableBlock pBlock)
        {
            pBlock.onPick -= DeletePickableProjection;
            
            _pool.Push(_pickableProjections[pBlock].projection);
            _pickableProjections.Remove(pBlock);
            
            UpdateProjectionLines();
        }
        
    }
}