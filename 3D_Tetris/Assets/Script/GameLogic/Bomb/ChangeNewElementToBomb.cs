using System.Collections.Generic;
using Helper.Patterns;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.GameLogic.Bomb
{
    public class ChangeNewElementToBomb : MonoBehaviour
    {
        private ElementData _elementData;
        private GameLogicPool _gameLogicPool;
        private BombsManager _bombsManager;
        
        [SerializeField] private GameObject _particleSystem;
        [SerializeField] private Transform _particlesParent;
        [SerializeField] private float _timeForShowStop = 0.5f;
        
        private Pool<GameObject> _particlePool;
        private List<GameObject> _activeParticles;
        
        public void Start()
        {
            _elementData = ElementData.Instance;
            _gameLogicPool = RealizationBox.Instance.gameLogicPool;
            _bombsManager = RealizationBox.Instance.bombsManager;
            
            _particlePool = new Pool<GameObject>(_particleSystem,_particlesParent);
            _activeParticles = new List<GameObject>();
        }
        
        public void ChangeToBomb(bool isBig)
        {
            var deleteBlocks = new List<Block>(_elementData.newElement.blocks);
            deleteBlocks.RemoveAt(deleteBlocks.Count - 1);

            OnDestroyBlock(_elementData.newElement.blocks);
            _elementData.newElement.RemoveBlocksInList(deleteBlocks.ToArray());
            foreach (var block in deleteBlocks)
            {
                _gameLogicPool.DeleteBlock(block);
            }
            
            _bombsManager.AddBomb(_elementData.newElement.blocks[0],isBig);
            
            RealizationBox.Instance.projection.CreateProjection();
            RealizationBox.Instance.projectionLineManager.UpdateProjectionLines();
        }
        
        public void OnDestroyBlock(List<Block> pos)
        {
            foreach (var po in pos)
            {
                var boom = _particlePool.Pop();
                boom.transform.position = po.myTransform.position;
                _activeParticles.Add(boom);
            }
            Invoke(nameof(DestroyParticle), _timeForShowStop);
        }

        public void DestroyParticle()
        {
            foreach (var ap in _activeParticles)
            {
                _particlePool.Push(ap);
            }
            _activeParticles.Clear();
        }
    }
}