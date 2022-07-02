using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
       public int _currentStepSave = 1000;
       
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

       private int _isOpenedBox = 0;
       public int isOpenedBox => _isOpenedBox;

       private Sequence _uiBoxAnimation;
       [SerializeField] private RectTransform _evilBoxRectTransform;
       [SerializeField] private RectTransform _evilBoxParentRectTransform;
       [SerializeField] private CanvasGroup _oreol;
       [SerializeField] private float _time = 0.7f;
       [SerializeField] private float _deltaMove;
       [SerializeField] private float _timeMoving = 0.2f;
       private float _rotation;
       [SerializeField] private RectTransform _oreolTransform;

       
       // 3d animation
       [SerializeField] private Canvas _canvas;
       [SerializeField] private Transform _centerPoint;
       [SerializeField] private float _timeBlockmove = 1f;
       private int _blockCompleteCounter;
       private List<Block> _animationBlocks;
       

       [SerializeField] private int _minBox, _maxBox;
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
            _animationBlocks = new List<Block>();
            
            _boxes = new List<Block>();
            _particles2 = new Dictionary<Block, GameObject>();

            CanvasGroup boxCanvas = _evilBoxRectTransform.gameObject.GetComponent<CanvasGroup>();
            _uiBoxAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            _uiBoxAnimation
                .Append(_evilBoxRectTransform.DOAnchorPosY(170, _time / 2)
                    .From(Vector2.up * (-200)))
                .Join(boxCanvas.DOFade(1f, _time / 2.5f).From(0f))
                .Append(_evilBoxRectTransform.DOAnchorPosY(130, _timeMoving)
                    .From(Vector2.up * (170)).SetLoops(3, LoopType.Yoyo))
                .Join(_oreol.DOFade(1, _timeMoving / 4).From(0).OnComplete(() =>
                {
                    OpenEvilBoxLogic();
                    _uiBoxAnimation.Pause();
                }))
                //HIDE PART
                .Append(_oreol.DOFade(0, _time / 4).From(1))
                .Append(_evilBoxRectTransform.DOAnchorPosY(100, _time / 3))
                .Join(boxCanvas.DOFade(0f, _time / 3).From(1f));
               // .OnComplete( ()=>OnOpenBoxEnded?.Invoke());

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
     
        public void OnDestroyBox(Block box)
        {
            box.OnDestroyed -= OnDestroyBox;
            box.OnCollected -= OnCollectBox;
            DestroyParticle(box);
            _boxes.Remove(box);
        }

        public void OnCollectBox(Block box)
        {
            box.OnDestroyed -= OnDestroyBox;
            box.OnCollected -= OnCollectBox;
            DestroyParticle(box);
            _boxes.Remove(box);
            
            _isOpenedBox++;
        }

        public void UiBoxAnimation()
        {
            
        }

        public void DestroyParticle(Block box)
        {
            _particlePool.Push(_particles2[box]);
            _particles2.Remove(box);
        }
        
        public bool OpenEvilBox()
        {
            if (_isOpenedBox ==0)
                return false;
            
            int _minHeight = 17;
            int currentHeightPosition  = (_matrix.height - _minHeight) * _gameCamera.lastMaxCurrentHeight / _heightHandler.limitHeight + _minHeight; //(_matrix.height - _minHeight) * _heightHandler.currentHeight / _heightHandler.limitHeight + _minHeight;
            currentHeightPosition -= 7;
            _centerPoint.position = new Vector3(0, currentHeightPosition, 0);
            
            SetUIBox(_centerPoint.position);
            _uiBoxAnimation.Rewind();
            _uiBoxAnimation.Play();
            return true;
        }
      
        public void OpenEvilBoxLogic()
        {
            if (_isOpenedBox == 0)
                return;

            int amount = Random.Range(_minBox* _isOpenedBox,_maxBox* isOpenedBox);
            amount = amount >9? 9: amount; // sorryyyyyy for it
            
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

                RealizationBox.Instance.generator.SetRandomPositionForEvilBlox(element, usedPositions);
                
                int currentYpos = Random.Range( _matrix.MinHeightInCoordinates(element.blocks[0]._coordinates.x.ToIndex(), element.blocks[0]._coordinates.z.ToIndex()) + 1, _heightHandler.limitHeight + 1);
                element.InitializationAfterGeneric(currentYpos);
                element.myTransform.position = new Vector3(element.myTransform.position.x, pos.y + currentYpos - size, element.myTransform.position.z);
                
                ElementData.Instance.MergeElement(element);
                
                usedPositions.Add(element.blocks[0].xz);
                blocks.Add(element.blocks[0]);
                Debug.Log("new block x: " + element.blocks[0].xz.x + " z: " + element.blocks[0].xz.z);
                element.blocks[0].mesh.enabled = false;
            }

            BlockAnimations(blocks);
           // OnAddBlock(blocks);
            _isOpenedBox = 0;
        }

        private void BlockAnimations(List<Block> blocks)
        {
            _animationBlocks.Clear();
            _animationBlocks = blocks;

            OnBlockAnimationSequence();
        }

        public void OnBlockAnimationSequence()
        {
            if (_animationBlocks.Count > 0)
            {
                var b = _animationBlocks[0];
                b.mesh.enabled = true;
                
                var time = Vector3.Distance(_centerPoint.position, b.transform.position) / 10 * _timeBlockmove;

                b.transform.DOMove(b.transform.position,  time).From(_centerPoint.position)
                    .OnComplete(()=>OnBlockAnimationSequence()).OnUpdate(() =>
                        {
                            _rotation += Time.deltaTime * _starRotationSpeed;
                            if (_rotation > 360.0f)
                            {
                                _rotation = 0.0f;
                            }

                            _oreolTransform.localRotation = Quaternion.Euler(0, 0, _rotation);
                        });
             //   b.transform.DOScale(b.transform.localScale, time).From(Vector3.one * 0.3f);
                OnAddBlock(b);
                
                _animationBlocks.Remove(b);
            }
            else
            {
                DestroyParticle();
                _uiBoxAnimation.Play();
                OnOpenBoxEnded?.Invoke();
            }
        }
        
        public void OnAddBlock(Block block)
        {
            var boom = _createBlockParticlePool.Pop();
            boom.transform.parent = block.myTransform;
            boom.transform.localPosition = Vector3.zero;
                
            _createBlockActiveParticles.Add(boom);
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
            _currentStep = _currentStepSave;
            foreach (var box in _boxes)
            {
                box.OnDestroyed -= OnDestroyBox;
                box.OnCollected -= OnCollectBox;
                DestroyParticle(box);
            }
            _boxes.Clear();
        }
        
        
        public void SetUIBox(Vector3 pos)
        {
            _evilBoxParentRectTransform.anchoredPosition = WorldToCanvas(pos);
        }
        
        private Vector2 WorldToCanvas(Vector3 world_position)
        {
            var viewport_position = Camera.main.WorldToViewportPoint(world_position);
            var canvas_rect = _canvas.GetComponent<RectTransform>();
        
            return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
        }
    }
}