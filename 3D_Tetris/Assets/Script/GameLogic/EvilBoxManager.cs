using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Script.GameLogic
{
    public class EvilBoxManager: MonoBehaviour
    {
        private GameLogicPool _pool;
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;
        private HeightHandler _heightHandler;
        private GameCamera _gameCamera;
        
        public Action OnOpenBoxEnded;
        
        public bool lvlWithEvilBox { get; set; }
        
        [SerializeField]  private Material _boxBlocksMaterial;
       [FormerlySerializedAs("_material")] [SerializeField] private Material _blockMaterial;
       [SerializeField] private Material _boxMaterial;
       [SerializeField] private Mesh _boxMesh;

       private List<Block> _boxes;
       private Dictionary<Block, GameObject> _particles2;

       [SerializeField] public int _stepForBomb;

       [SerializeField] private float _timeForShowStop = 0.5f;
     
       [SerializeField] private GameObject _particleSystem;
       [SerializeField] private GameObject _createBlockParticleSystem;
       
       [SerializeField] private Transform _particlesParent;

       [SerializeField] private int _minBlocksInBox;
       
       [SerializeField] private int _maxBlocksInBox;

       
       public int _currentStep = 1000;
       
       private Pool<GameObject> _createBlockParticlePool;
       private Pool<GameObject> _particlePool;
       private List<GameObject> _activeParticles;
       private List<GameObject> _createBlockActiveParticles;

       private Sequence _boomTextAnimation;

       [SerializeField] private Vector3 _boxRotation;

       private float oreolAngle = 0;
       
       // particles
       [SerializeField] private GameObject _particles;
       [SerializeField] private Vector3 _localParticlePosition;
       [SerializeField] private float _starRotationSpeed = 20.0f;

       private bool _isOpenedBox = false;
       public bool isOpenedBox => _isOpenedBox;

       private Sequence _uiBoxAnimation;
       [SerializeField] private RectTransform _evilBoxRectTransform;
       [SerializeField] private CanvasGroup _oreol;
       [SerializeField] private float _time = 0.7f;
       [SerializeField] private float _deltaMove;
       [SerializeField] private float _timeMoving = 0.2f;
       private float _rotation;
       [SerializeField] private RectTransform _oreolTransform;
       private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;
            _matrix = RealizationBox.Instance.matrix;
            _gameCamera = RealizationBox.Instance.gameCamera;
            _heightHandler = RealizationBox.Instance.haightHandler;
            
            _particlePool = new Pool<GameObject>(_particleSystem,_particlesParent);
            _createBlockParticlePool = new Pool<GameObject>(_createBlockParticleSystem,_particlesParent);
            _activeParticles = new List<GameObject>();
            _createBlockActiveParticles = new List<GameObject>();

            _boxes = new List<Block>();
            _particles2 = new Dictionary<Block, GameObject>();

            CanvasGroup boxCanvas = _evilBoxRectTransform.gameObject.GetComponent<CanvasGroup>();
            _uiBoxAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _uiBoxAnimation 
                .Append(_evilBoxRectTransform.DOAnchorPosY(Screen.height / 2 + 100, _time / 2).From(Vector2.up * (Screen.height / 2 - 400)))
                .Join(boxCanvas.DOFade(1f, _time / 2.5f).From(0f))
                .Append(_evilBoxRectTransform.DOAnchorPosY(Screen.height / 2 + 100, _timeMoving)
                    .From(Vector2.up *(Screen.height / 2)).SetLoops(3, LoopType.Yoyo))
                .Join(_oreol.DOFade(1,_timeMoving/4 ).From(0).OnComplete(() =>
                {
                    OpenEvilBoxLogic();
                }))
                //HIDE PART
                .Append(_oreol.DOFade(0,_time/4 ).From(1))
                .Append(_evilBoxRectTransform.DOAnchorPosY(Screen.height / 2 + 100, _time/3))
                .Join(boxCanvas.DOFade(0f, _time/3).From(1f))
                .OnComplete( ()=>OnOpenBoxEnded?.Invoke());

            _uiBoxAnimation.OnUpdate(() =>
            {
                _rotation += Time.deltaTime * _starRotationSpeed;
                if (_rotation > 360.0f)
                {
                    _rotation = 0.0f;
                }

                _oreolTransform.localRotation = Quaternion.Euler(0, 0, _rotation);
            });
            _uiBoxAnimation.Complete();
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

            _isOpenedBox = true;
        }

        public void UiBoxAnimation()
        {
            
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
        
        public bool OpenEvilBox()
        {
            if (!_isOpenedBox)
                return false;
             
            
            _uiBoxAnimation.Rewind();
            _uiBoxAnimation.Play();
            return true;
        }
      
        public void OpenEvilBoxLogic()
        {
            if (!_isOpenedBox)
                return;

            _isOpenedBox = false;
            
            int amount = 3;
            List<CoordinatXZ> usedPositions = new List<CoordinatXZ>();
            List<Block> blocks = new List<Block>();
            
            for (int i = 0; i < amount; i++)
            {
                var element = _pool.CreateEmptyElement();
                _pool.CreateBlock(Vector3Int.zero, element, _boxBlocksMaterial);
                
                var pos = RealizationBox.Instance.elementDropper.transform.position;
                
                // выравниваем элемент относительно координат y 
                var min_y = element.blocks.Min(s => s.coordinates.y);
                var max_y = element.blocks.Max(s => s.coordinates.y);

                var size = max_y - min_y;

                int currentYpos = _heightHandler.limitHeight + 1;
                element.InitializationAfterGeneric(currentYpos);
                element.myTransform.position = new Vector3(pos.x, pos.y + currentYpos - size, pos.z);

                RealizationBox.Instance.generator.SetRandomPosition(element, usedPositions);
                ElementData.Instance.MergeElement(element);
                
                usedPositions.Add(element.blocks[0].xz);
                blocks.Add(element.blocks[0]);
            }

            OnAddBlock(blocks);
        }
        
        public void OnAddBlock(List<Block> pos)
        {
            foreach (var po in pos)
            {
                var boom = _createBlockParticlePool.Pop();
                boom.transform.position = po.myTransform.position;
                _createBlockActiveParticles.Add(boom);
            }
            Invoke(nameof(DestroyParticle), _timeForShowStop);
        }

        public void DestroyParticle()
        {
            foreach (var ap in _createBlockActiveParticles)
            {
                _createBlockParticlePool.Push(ap);
            }
            _createBlockActiveParticles.Clear();
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