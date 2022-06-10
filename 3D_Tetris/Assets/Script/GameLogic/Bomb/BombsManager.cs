using System;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
using Helper.Patterns;
using IntegerExtension;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.GameLogic.Bomb
{
    public class BombsManager : MonoBehaviour
    {   
        private GameLogicPool _pool;
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;
        
        public Action OnBoomEnded;
        
        public bool lvlWithBombs { get; set; }
        
       [FormerlySerializedAs("_material")] [SerializeField] private Material _blockMaterial;
       [SerializeField] private Material _bombMaterial;
       [SerializeField] private Mesh _bombMesh;

       [SerializeField] private bool _ignoreSlow = true;

       private Block _bomb, _bigBomb;

       [SerializeField] private List<Vector3Int> _directions;
       [SerializeField] private int _destroyLayersAmount = 2; //For Big Bomb
       
       [SerializeField] public int _stepForBomb;

       [SerializeField] private float _timeForShowStop = 0.5f;
     
       [SerializeField] private GameObject _particleSystem;

       [SerializeField] private Transform _particlesParent;
           
       public int _currentStep = 1000;
       
       private Pool<GameObject> _particlePool;
       private List<GameObject> _activeParticles;

       [SerializeField] private CanvasGroup _boomText;
       private RectTransform rt;
       private Sequence _boomTextAnimation;
       [SerializeField] private float firstMove = 40;
       [SerializeField] private float firstMoveTime = 1;

       [SerializeField] private float dissapeadScale;
       [SerializeField] private float dissapeadScaleTime;

       [SerializeField] private Canvas _canvas;

       [SerializeField] private Vector3 _bombRotation;

       // particles
       [SerializeField] private GameObject _particles;
       [SerializeField] private Vector3 _localParticlePosition;

       //BB-Text for big bomb
       [SerializeField] private GameObject _BBText;
       [SerializeField] private Vector3 _localBBTextPosition;
       
       private Transform _gameCamera;
       
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;
            _matrix = RealizationBox.Instance.matrix;
            _gameCamera = RealizationBox.Instance.gameCamera.lookAtPoint;
            
            _particlePool = new Pool<GameObject>(_particleSystem,_particlesParent);
            _activeParticles = new List<GameObject>();
            
            _matrix.OnDestroyBlock += OnDestroyBlock;
            
           _boomTextAnimation = DOTween.Sequence().SetAutoKill(false).Pause();
            
            rt =  _boomText.gameObject.GetComponent<RectTransform>();
        
            _boomTextAnimation.Append( _boomText.DOFade(1, firstMoveTime / 2).From(0, false))
                .Append( _boomText.DOFade(0, dissapeadScaleTime))
                .Join(rt.DOScale(Vector3.one * dissapeadScale, dissapeadScaleTime))
                .OnComplete(() => rt.localScale = Vector3.one);
        }

        public Element MakeBomb(bool isBig = false)
        {
            _currentStep = 0;
            
            var element = _pool.CreateEmptyElement();
            _pool.CreateBlock(Vector3Int.zero, element, _blockMaterial);
            
            AddBomb(element.blocks[0], isBig);
            
            return element;
        }
        
        public bool CanMakeBomb()
        {
            if (!lvlWithBombs)
                return false;
            
            if (_currentStep < _stepForBomb)
            {
                _currentStep++;
                return false;
            }

            return true;
        }

        public void AddBomb(Block bomb, bool isBig)
        {
            bomb.TransformToBomb(_bombMesh, _bombMaterial, _blockMaterial,_bombRotation, isBig);

            _particles.SetActive(true);
            _particles.transform.parent = bomb.Star;
            _particles.transform.localPosition = _localParticlePosition;

            if (isBig)
            {
                _BBText.SetActive(true);
                _BBText.transform.SetParent(bomb.Star);
                _BBText.transform.localPosition = _localBBTextPosition;
                _BBText.transform.localRotation = Quaternion.Euler(0, 180, 0);
                
                _bigBomb = bomb;
            }   
            else
                _bomb = bomb;
        }
        
        private void Update()
        {
            if (Equals(_bomb, null) && Equals(_bigBomb, null))
                return;

            if(!Equals(_bomb, null)) 
                _bomb.Star.LookAt(_gameCamera);
            else
                _bigBomb.Star.LookAt(_gameCamera);
        }

        public bool BoomBombs()
        {
            bool boom = false;
            
            if(!Equals(_bomb,null))
            {
                RealizationBox.Instance.matrix.DestroyBlocksAround(_bomb._coordinates.ToIndex(), _directions,true);
                boom = true;
            }
            else if (!Equals(_bigBomb, null))
            {
                RealizationBox.Instance.matrix.DestroyBlocksInLayers(_bigBomb._coordinates.ToIndex(), _destroyLayersAmount, true);
                boom = true;
            }
            
            if(!boom)
                OnBoomEnded?.Invoke();
            else
            {
                Invoke(nameof(BoomAnimationEnd), _timeForShowStop);
            }
            
            _bomb = _bigBomb = null;
            return boom;
        }

        public void BoomAnimationEnd()
        {
            OnBoomEnded?.Invoke();
        }

        public void OnDestroyBlock(List<Vector3> pos)
        {
            foreach (var po in pos)
            {
                var boom = _particlePool.Pop();
                boom.transform.position = po;
                _activeParticles.Add(boom);
            }

            SetBoomText(pos[pos.Count - 1]);
            
            _particles.SetActive(false);
            _particles.transform.SetParent(transform);
            
            _BBText.SetActive(false);
            _BBText.transform.SetParent(transform);
            _BBText.transform.localScale = Vector3.one;
            
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

        public void SetBoomText(Vector3 pos)
        {
            rt.anchoredPosition = WorldToCanvas(pos) + new Vector2(0,firstMove);

            _boomTextAnimation.Rewind();
            _boomTextAnimation.Play();
        }
        
        private Vector2 WorldToCanvas(Vector3 world_position)
        {
            var viewport_position = Camera.main.WorldToViewportPoint(world_position);
            var canvas_rect = _canvas.GetComponent<RectTransform>();
        
            return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
                (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
        }

        public void Clear()
        {
            _bomb = null;
            _bigBomb = null;
        }
    }
}