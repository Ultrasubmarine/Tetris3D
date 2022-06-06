using System;
using System.Collections.Generic;
using DG.Tweening;
using Helper.Patterns;
using IntegerExtension;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.GameLogic
{
    public class EvilBoxManager: MonoBehaviour
    {
        private GameLogicPool _pool;
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;
        
        public Action OnBoomEnded;
        
        public bool lvlWithEvilBox { get; set; }
        
       [FormerlySerializedAs("_material")] [SerializeField] private Material _blockMaterial;
       [SerializeField] private Material _boxMaterial;
       [SerializeField] private Mesh _boxMesh;

       private List<Block> _boxes;
       private Dictionary<Block, GameObject> _particles2;

       [SerializeField] public int _stepForBomb;

       [SerializeField] private float _timeForShowStop = 0.5f;
     
       [SerializeField] private GameObject _particleSystem;

       [SerializeField] private Transform _particlesParent;
           
       private int _currentStep = 1000;
       
       private Pool<GameObject> _particlePool;
       private List<GameObject> _activeParticles;

       private Sequence _boomTextAnimation;

       [SerializeField] private Vector3 _boxRotation;

       private float oreolAngle = 0;
       
       // particles
       [SerializeField] private GameObject _particles;
       [SerializeField] private Vector3 _localParticlePosition;
       [SerializeField] private float _starRotationSpeed = 20.0f;
       
       private Transform _gameCamera;
       
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;
            _matrix = RealizationBox.Instance.matrix;
            _gameCamera = RealizationBox.Instance.gameCamera.lookAtPoint;
            
            _particlePool = new Pool<GameObject>(_particleSystem,_particlesParent);
            _activeParticles = new List<GameObject>();
          
            _boxes = new List<Block>();
            _particles2 = new Dictionary<Block, GameObject>();
        }

        public Element MakeEvilBox()
        {
            _currentStep = 0;
            
            var element = _pool.CreateEmptyElement();
            _pool.CreateBlock(Vector3Int.zero, element, _blockMaterial);
            
            AddEvilBox(element.blocks[0]);
            
            return element;
        }
        
        public bool CanMakeEvilBox()
        {
            if (!lvlWithEvilBox)
                return false;
            
            if (_currentStep < _stepForBomb)
            {
                _currentStep++;
                return false;
            }

            return true;
        }

        public void IncrementStep()
        {
            _currentStep++;
        }

        private void Update()
        {
            if (_boxes.Count == 0) return;
            
            oreolAngle += Time.deltaTime *_starRotationSpeed;
            if (oreolAngle > 360.0f)
            {
                oreolAngle = 0.0f;
            }
            foreach (var box in _boxes)
            {
                box.oreol.localRotation = Quaternion.Euler(90, 0, oreolAngle);
            }
        }

        public void AddEvilBox(Block box)
        {
            box.TransformToBox(_boxMesh, _boxMaterial, _blockMaterial,_boxRotation);
            
            var boom = _particlePool.Pop();
            _activeParticles.Add(boom);
            
            boom.transform.parent = box.Star;
            boom.transform.localPosition = _localParticlePosition;
            _particles2.Add(box, boom);
            // _BBText.SetActive(true);
            // _BBText.transform.SetParent(box.Star);
            // _BBText.transform.localPosition = _localBBTextPosition;
            // _BBText.transform.localRotation = Quaternion.Euler(0, 180, 0);
            //
            box.OnDestroyed += OnDestroyBox;
            box.OnCollected += OnCollectBox;
            
            _boxes.Add(box);
        }
        
        // private void Update()
        // {
        //     if (Equals(_bomb, null) && Equals(_box, null))
        //         return;
        //
        //     if(!Equals(_bomb, null)) 
        //         _bomb.Star.LookAt(_gameCamera);
        //     else
        //         _box.Star.LookAt(_gameCamera);
        // }


        public void OnDestroyBox(Block box)
        {
            box.OnDestroyed -= OnDestroyBox;
            box.OnCollected -= OnCollectBox;
            DestroyParticle(box);
        }

        public void OnCollectBox(Block box)
        {
            box.OnDestroyed -= OnDestroyBox;
            box.OnCollected -= OnCollectBox;
            DestroyParticle(box);
        }
        
        // public bool BoomBombs()
        // {
        //     bool boom = false;
        //     
        //     if(!Equals(_bomb,null))
        //     {
        //         RealizationBox.Instance.matrix.DestroyBlocksAround(_bomb._coordinates.ToIndex(), _directions,true);
        //         boom = true;
        //     }
        //     else if (!Equals(_box, null))
        //     {
        //         RealizationBox.Instance.matrix.DestroyBlocksInLayers(_box._coordinates.ToIndex(), _destroyLayersAmount, true);
        //         boom = true;
        //     }
        //     
        //     if(!boom)
        //         OnBoomEnded?.Invoke();
        //     else
        //     {
        //         Invoke(nameof(BoomAnimationEnd), _timeForShowStop);
        //     }
        //     
        //     _bomb = _box = null;
        //     return boom;
        // }
        //
        // public void BoomAnimationEnd()
        // {
        //     OnBoomEnded?.Invoke();
        // }

        // public void OnDestroyBlock(List<Vector3> pos)
        // {
        //     foreach (var po in pos)
        //     {
        //         var boom = _particlePool.Pop();
        //         boom.transform.position = po;
        //         _activeParticles.Add(boom);
        //     }
        //
        //     SetBoomText(pos[pos.Count - 1]);
        //     
        //     _particles.SetActive(false);
        //     _particles.transform.SetParent(transform);
        //     
        //     _BBText.SetActive(false);
        //     _BBText.transform.SetParent(transform);
        //     _BBText.transform.localScale = Vector3.one;
        //     
        //     Invoke(nameof(DestroyParticle), _timeForShowStop);
        // }

        public void DestroyParticle(Block box)
        {
            _particlePool.Push(_particles2[box]);
            _particles2.Remove(box);
        }

        public void Clear()
        {
            foreach (var box in _boxes)
            {
                box.OnDestroyed -= OnDestroyBox;
                box.OnCollected -= OnCollectBox;
                DestroyParticle(box);
            }
            _boxes.Clear();
        }
    }
}