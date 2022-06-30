using System;
using System.Collections.Generic;
using System.Configuration;
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

        [SerializeField] private int _stoneLives = 3;
        [SerializeField] private List<Mesh> _livesMaterial;
        [SerializeField] private List<Mesh> _livesStarMaterial;
        
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
                block.TransformToStone(_stoneCellMesh, _cellMaterial,_blockMaterial, _stoneLives);
                
                block.OnDestroyed += OnDestroyStoneBlock;
                block.OnCollected += OnDestroyStoneBlock;
                
                block.OnDamaged += OnDamageStoneBlock;
                _stoneBlocks.Add(block);
            }
        }

        public void OnDestroyStoneBlock(Block block)
        {
            block.OnDestroyed -= OnDestroyStoneBlock;
            block.OnCollected -= OnDestroyStoneBlock;
            
            block.OnDamaged -= OnDamageStoneBlock;
            _stoneBlocks.Remove(block);
        }

        public void OnDamageStoneBlock(Block block)
        {
            if (block.lives < 1) 
                return;
            
            if(block.isStar)
                block.meshFilter.mesh = GetStoneMesh(block.lives-1,true);
            else
                block.extraMeshFilter.mesh = GetStoneMesh(block.lives-1);
        }
        
        public Mesh GetStoneMesh(int lives, bool star = false)
        {
            if (star)
            {
                if (_livesStarMaterial.Count <= lives)
                    return null;

                return _livesStarMaterial[lives];
            }
            else
            {
                if (_livesMaterial.Count <= lives)
                    return null;

                return _livesMaterial[lives]; 
            }
        }
        
        public void Clear()
        {
            _currentStep = _currentStepSave;
            
            foreach (var block in _stoneBlocks)
            { 
                block.OnDestroyed -= OnDestroyStoneBlock;
                block.OnCollected -= OnDestroyStoneBlock;
                
                block.OnDamaged -= OnDamageStoneBlock;
            }
            _stoneBlocks.Clear();
        }
    }
}