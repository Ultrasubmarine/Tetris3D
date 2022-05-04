using System;
using System.Collections.Generic;
using System.Configuration;
using DG.Tweening;
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
        
        public bool lvlWithBombs { get; set; }
        
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

       [SerializeField] private CanvasGroup _boomText;
       private RectTransform rt;
       private Sequence _boomTextAnimation;
       [SerializeField] private float firstMove = 40;
       [SerializeField] private float firstMoveTime = 1;

       [SerializeField] private float dissapeadScale;
       [SerializeField] private float dissapeadScaleTime;

       [SerializeField] private Canvas _canvas;
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;
            _matrix = RealizationBox.Instance.matrix;
            
            _bombs = new List<Block>();
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

        public Element MakeBomb()
        {
            if (!lvlWithBombs)
                return null;
            
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
            foreach (var po in pos)
            {
                var boom = _particlePool.Pop();
                boom.transform.position = po;
                _activeParticles.Add(boom);
            }

            SetBoomText(pos[pos.Count - 1]);
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
    }
}