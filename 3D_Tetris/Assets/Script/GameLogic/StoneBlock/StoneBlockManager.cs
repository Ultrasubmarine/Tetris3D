using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Script.GameLogic.StoneBlock
{
    public class StoneBlockManager: MonoBehaviour
    {
        public Material blockMaterial => _blockMaterial;
        public bool lvlWithStone = true;

        [SerializeField] private Mesh _stoneCellMesh;
        [SerializeField] private Material _cellMaterial;
        [SerializeField] private Material _blockMaterial;
        
        [SerializeField] private int _maxStoneBlocks; // ???
        [SerializeField] public int _stepForStoneBlock;
        public int _currentStep = 1000;
        public int _currentStepSave = 1000;
        
        private List<Block> _stoneBlocks;

        private void Awake()
        {
            _stoneBlocks = new List<Block>();
        }

        public bool CanTransformToStone()
        {
            if (!lvlWithStone)
                return false;
            
            if (_currentStep < _stepForStoneBlock)
            {
                _currentStep++;
                return false;
            }

            _currentStep = 0;
            return true;
        }
        
        public void TransformToStone(Element element)
        {
            foreach (var block in element.blocks)
            {
                block.TransformToStone(_stoneCellMesh, _cellMaterial,_blockMaterial);
                
                block.OnDestroyed += OnDestroyStoneBlock;
                _stoneBlocks.Add(block);
            }
        }

        public void OnDestroyStoneBlock(Block block)
        {
            block.OnDestroyed -= OnDestroyStoneBlock;
            _stoneBlocks.Remove(block);
        }

        public void Clear()
        {
            _currentStep = _currentStepSave;
            
            foreach (var block in _stoneBlocks)
            { 
                block.OnDestroyed -= OnDestroyStoneBlock;
            }
            _stoneBlocks.Clear();
        }
    }
}