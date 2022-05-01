using System;
using System.Collections.Generic;
using System.Configuration;
using Helper.Patterns;
using IntegerExtension;
using UnityEngine;

namespace Script.GameLogic.Bomb
{
    public class BombsManager : MonoBehaviour
    {   
        private GameLogicPool _pool;
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;
        
        public Action OnBoomEnded;
        
       [SerializeField] private Material _material;
       [SerializeField] private Mesh _bombMesh;

       [SerializeField] private bool _ignoreSlow = true;

       private List<Block> _bombs;

       [SerializeField] private List<Vector3Int> _directions;

       [SerializeField] private int _stepForBomb;

       [SerializeField] private float _timeForShowStop = 0.5f;
     
       [SerializeField] private GameObject _particleSystem;

       [SerializeField] private Transform _particlesParent;
           
       private int _currentStep = 1000;
       
       private Pool<GameObject> _particlePool;
       private List<GameObject> _activeParticles;
       
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;
            _matrix = RealizationBox.Instance.matrix;
            
            _bombs = new List<Block>();
            _particlePool = new Pool<GameObject>(_particleSystem,_particlesParent);
            _activeParticles = new List<GameObject>();
        }

        public Element MakeBomb()
        {
            if (_currentStep < _stepForBomb)
            {
                _currentStep++;
                return null;
            }
            else
            {
                _currentStep = 0;
            }
            
            var element = _pool.CreateEmptyElement();
            _pool.CreateBlock(Vector3Int.zero, element, _material);

            element.blocks[0].TransformToBomb(_bombMesh, _material);
            _bombs.Add(element.blocks[0]);
            return element;
        }

        public bool BoomBombs()
        {
            _matrix.OnDestroyBlock += OnDestroyBlock;
            bool boom = false;
            foreach (var b in _bombs)
            {
                RealizationBox.Instance.matrix.DestroyBlocksAround(b._coordinates.ToIndex(), _directions);
                boom = true;
            }
            
            if(_bombs.Count == 0)
                OnBoomEnded?.Invoke();
            else
            {
                Invoke(nameof(BoomAnimationEnd), _timeForShowStop);
            }
            _bombs.Clear();
            return boom;
        }

        public void BoomAnimationEnd()
        {
            OnBoomEnded?.Invoke();
        }

        public void OnDestroyBlock(List<Vector3> pos)
        {
            _matrix.OnDestroyBlock -= OnDestroyBlock;
            
            foreach (var po in pos)
            {
                var boom = _particlePool.Pop();
                boom.transform.position = po;
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