using System.Collections.Generic;
using System.Linq;
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

        public void ChangeToBigBomb(bool isBig)
        {
            // clear old
            var deleteBlocks = new List<Block>(_elementData.newElement.blocks);
            OnDestroyBlock(_elementData.newElement.blocks);
            _elementData.newElement.RemoveBlocksInList(deleteBlocks.ToArray());
            foreach (var block in deleteBlocks)
            {
                _gameLogicPool.DeleteBlock(block);
            }
            _gameLogicPool.DeleteElement(_elementData.newElement);
            RealizationBox.Instance.influenceManager.ClearAllInfluences();
            
            // create new
            var element = _gameLogicPool.CreateEmptyElement();
            _gameLogicPool.CreateBlock(new Vector3Int(1,0,1), element, null);
            _elementData.CustomSetNewElement(element);
               
            var parentTransform  = RealizationBox.Instance.elementDropper.transform;

            // выравниваем элемент относительно координат y 
            var min_y = element.blocks.Min(s => s.coordinates.y);
            var max_y = element.blocks.Max(s => s.coordinates.y);

            var size = max_y - min_y;

            var pos = RealizationBox.Instance.elementDropper.transform.position;
            var _matrix = RealizationBox.Instance.matrix;
            int currentHeightPosition  = (_matrix.height - 17) * RealizationBox.Instance.gameCamera.lastMaxCurrentHeight / RealizationBox.Instance.haightHandler.limitHeight + 17; //(_matrix.height - _minHeight) * _heightHandler.currentHeight / _heightHandler.limitHeight + _minHeight;
       
            
            element.myTransform.parent = parentTransform;
            
            element.InitializationAfterGeneric(currentHeightPosition);
            element.myTransform.position = new Vector3(pos.x, pos.y + currentHeightPosition - size, pos.z);
            ///
            /// 
            _bombsManager.AddBomb(element.blocks[0],isBig);
            
            RealizationBox.Instance.FSM.SetNewState(TetrisState.Drop);
            
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