using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using IntegerExtension;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.GameLogic.Stars
{
    class StarInfo
    {
        public Block block;
        public float angle;

        public StarInfo(Block block, float angle)
        {
            this.block = block;
            this.angle = angle;
        }
    }

    [Serializable]
    public struct StarPlace
    {
        public int amountStars;
        public Vector3Int position;

        public void increment(int amnt)
        {
            amountStars = amnt;
        }
    }
    
    public class StarsManager : MonoBehaviour
    {
        public bool collectStarLvlLvl { get { return _collectStarsLvl; } set 
        { _collectStarsLvl = value; 
            if (!_collectStarsLvl) {RealizationBox.Instance.starUI.gameObject.SetActive(false);}
        } }


        public bool onlyStarPlace;
        public bool allPlaceInFirstStep;
        
        public int collectedStars { get; private set; }
        public int neededStars { get { return _neededStars;} set { _neededStars = value; } }
        public int maxStarsAmount { get { return _maxStarsAmount;} set { _maxStarsAmount = value; } }
        
        [SerializeField] private bool _collectStarsLvl;
        [SerializeField] private int _maxStarsAmount = 1;
        [SerializeField] private int _neededStars = 5;
        
        public Action OnCreatedStar;
        public Action OnUpdatedCollectingStars;
        public Action OnCollectedStars;
        public bool onCollectedAnimationWaiting { get; private set; }

        public int stepsBetweenStar { get; set; }
        [FormerlySerializedAs("_currentStep")] public int currentStep;
        
        [SerializeField] private Mesh _starMesh;
        [FormerlySerializedAs("_starmaterial")] [SerializeField] private Material _blockMaterial;
        [SerializeField] private Material _starMaterial;

        // Animation
        [Header("Animation")]
        [SerializeField] private Transform _rotationStar;
        [SerializeField] private Transform _animationStar;
        [SerializeField] private TrailRenderer _starTail;
        [SerializeField] private float _highestPathPos = 32;

        [SerializeField] private float _firstPointR = 2.5f;
        [SerializeField] private float _secondPointR = 2.5f;
        [SerializeField] private float _secondPointYP = 2.5f;
        private List<Vector3> _animationPath;
        
        [SerializeField] private float _starRotationSpeed = 20.0f;
        [SerializeField] private float _fallStarRotationSpeed = 300.0f;
        private List<StarInfo> _rotationStars;
        private float _fallStarAngle = 0;
        
        [SerializeField] private ParticleSystem _particles;
        private bool isParticle = false;
        // End of animation
        
        private List<Block> _stars;
        private List<Block> _applicants;
        
        private PlaneMatrix _matrix;
        private Transform _cameraTransform;

        public List<StarPlace> starPlaces { get; set; }
        
        [SerializeField] private Transform _lookAtStar;
      
        private void Start()
        {
          //  currentStep = 1000;
            _rotationStars = new List<StarInfo>();
            _stars = new List<Block>();
            _applicants = new List<Block>();
            _animationPath = new List<Vector3>() {new Vector3(0, _highestPathPos, 0), Vector3.zero};
            
            _matrix = RealizationBox.Instance.matrix;
            _cameraTransform = RealizationBox.Instance.gameCamera.transform;
        }

        private void Update()
        {
            foreach (var s in _rotationStars)
            {
                s.block.Star.LookAt(Camera.main.transform);
                s.angle += Time.deltaTime *_starRotationSpeed;
                if (s.angle > 360.0f)
                {
                    s.angle = 0.0f;
                }
                s.block.oreol.localRotation = Quaternion.Euler(0, 0, s.angle);
            }
            
            
        }

        public bool CanCreateStar()
        {
            if (_stars.Count >= _maxStarsAmount)
                return false;

            _applicants.Clear();
            for (int i = 0; i <= _matrix.height; i++)
            {
                if (_stars.Exists((Block b) => { return b._coordinates.y == i; }))
                    continue;

                AddApplicants(0, i, 0);
                AddApplicants(0, i, _matrix.wight-1);
                AddApplicants(_matrix.wight-1, i, 0);
                AddApplicants(_matrix.wight-1, i, _matrix.wight-1);
            }

            bool starPlace = false;
            foreach (var p in starPlaces)
            {
                if (!_matrix.CheckEmptyPlace(p.position.x, p.position.y, p.position.z))
                {
                    starPlace = true;
                    break;
                }
            }

            if (!starPlace && onlyStarPlace)
            {
                return false;
            }
            if (_applicants.Count == 0 && !starPlace)
            {
                currentStep++;
                return false;
            }

            if (RealizationBox.Instance.gameManager.infinity && _stars.Count == 0)
            {
                currentStep = 0;
                return true;
            }
            else if (currentStep < stepsBetweenStar)
            {
                currentStep++;
                return false;
            }
            else
            {
                currentStep = 0;
                return true;
            }
        }
        
        void AddApplicants(int x, int y, int z)
        {
            var b = _matrix.GetBlockInPlace(x, y, z);
            if (b)
            {   
                _applicants.Add(b);    
            }
        }

        public void CreateStar()
        {  
            Block rndBlock; 
            
            int ind = -1;
            for (int i=0; i < starPlaces.Count; i++)
            {
                if (!_matrix.CheckEmptyPlace(starPlaces[i].position.x, starPlaces[i].position.y, starPlaces[i].position.z))
                {
                    ind = i;
                    break;
                }
            }

            if (ind != -1)
            {
                rndBlock = _matrix.GetBlockInPlace(starPlaces[ind].position.x, starPlaces[ind].position.y, starPlaces[ind].position.z);
                if (starPlaces[ind].amountStars - 1 == 0) 
                    starPlaces.RemoveRange(ind,1);
                else 
                    starPlaces[ind].increment(starPlaces[ind].amountStars-1); 
            }
            else
            {
                rndBlock = _applicants[Random.Range(0, _applicants.Count)];
            }
            
            _stars.Add(rndBlock); 
            rndBlock.OnCollected += CollectStar;
            rndBlock.OnDestroyed += CollectStar;//DestroyStar;
            CreateAnimation(rndBlock);
        }

        public void CreateAnimation(Block block)
        {
            isParticle = false;
            _animationStar.transform.localPosition = Vector3.zero;
            _animationStar.transform.localRotation = Quaternion.identity;
            
            Vector3 endPosition =  _animationStar.InverseTransformPoint(block.myTransform.position);
            
            _animationPath.Add(endPosition);
           _animationPath[0] = new Vector3(endPosition.x * _firstPointR, _animationPath[0].y, endPosition.z * _firstPointR);
           if(endPosition.x < 0 && endPosition.z < 0) // animation bug with star path
               _animationPath[1] = new Vector3((endPosition.x -1) * _secondPointR + 0.5f * _secondPointR  , endPosition.y + _secondPointYP, (endPosition.z - 1) * _secondPointR + 0.5f * _secondPointR );
           else
            _animationPath[1] = new Vector3(endPosition.x * _secondPointR + 0.5f * _secondPointR  , endPosition.y + _secondPointYP, endPosition.z * _secondPointR + 0.5f * _secondPointR );

            _animationStar.gameObject.SetActive(true);
            _animationStar.localPosition = _animationPath[0];//new Vector3(0, _highestPathPos, 0);//
            _animationStar.LookAt(_cameraTransform);//Camera.main.transform);
            _animationStar.DOLocalPath(_animationPath.ToArray(), 2, PathType.CatmullRom, PathMode.Ignore, gizmoColor : Color.green).OnPlay(()=>
                {
                    _rotationStar.gameObject.SetActive(true);
                    _starTail.Clear();
                }).
                    OnUpdate(() =>
                    {
                        _fallStarAngle += Time.deltaTime *_fallStarRotationSpeed;
                        if (_fallStarAngle > 360.0f)
                            {
                                _fallStarAngle = 0.0f;
                            }
                        _rotationStar.localRotation = Quaternion.Euler(0, 0, _fallStarAngle);
                        _lookAtStar.LookAt(_cameraTransform.position);

                        if (!isParticle && Vector3.Distance(_animationStar.transform.localPosition, _animationPath[2]) < 1)
                        {
                            _particles.transform.position = block.myTransform.position;
                            _particles.gameObject.SetActive(false);
                            _particles.gameObject.SetActive(true);

                            isParticle = true;
                        }
                    }).
              OnComplete(()=> 
              {
                  block.TransformToStar(_starMesh, _starMaterial,_blockMaterial);
                  
                  _animationPath.RemoveAt(2);
                  _animationStar.DOKill();
                  
               //   _starTail.SetActive(false);
                  _rotationStar.gameObject.SetActive(false);
                  _starTail.Clear();
                  _rotationStars.Add(new StarInfo(block, 0));
                  
                  OnCreatedStar?.Invoke();
              });

            //_animationStar.DOLocalRotate(Vector3.forward, 4).SetLoops(-1,LoopType.Incremental);
        }

        public void CollectStar(Block star)
        {
            star.OnCollected -= CollectStar;
            star.OnDestroyed -= CollectStar;//DestroyStar;
            collectedStars++;
            _stars.Remove(star);

            StarInfo rStr;
            foreach (var s in _rotationStars)
            {
                if (s.block == star)
                {
                    rStr = s;
                    _rotationStars.Remove(rStr);
                    break;
                }
            }

            //  OnUpdatedCollectingStars?.Invoke();
            if (!onCollectedAnimationWaiting)
            {
                onCollectedAnimationWaiting = true;
                RealizationBox.Instance.starUIAnimation.OnAnimationEnd += FinishCollectAnimation;
             }
            RealizationBox.Instance.starUIAnimation.StartAnimation();
        }

        public void DestroyStar(Block star)
        {
            star.OnCollected -= CollectStar;
            star.OnDestroyed -= CollectStar;//DestroyStar;
            
            _stars.Remove(star);

            StarInfo rStr;
            foreach (var s in _rotationStars)
            {
                if (s.block == star)
                {
                    rStr = s;
                    _rotationStars.Remove(rStr);
                    break;
                }
            }
        }
        public void FinishCollectAnimation() // doesnt work when few stars are collected
        {
            onCollectedAnimationWaiting = false;
            RealizationBox.Instance.starUIAnimation.OnAnimationEnd -= FinishCollectAnimation;
            
            OnCollectedStars?.Invoke();
        }

        public void Clear()
        {
            foreach (var s in _stars)
            {
                s.OnCollected -= CollectStar;
                s.OnDestroyed -= DestroyStar;
            }
            _stars.Clear();
            _applicants.Clear();
            collectedStars = 0;
            currentStep = 1000;
            _animationStar.DORewind();
            _animationStar.DOComplete();
            _particles.gameObject.SetActive(false);
            
            OnUpdatedCollectingStars?.Invoke();
        }

        public bool CheckWin()
        {
            if (!collectStarLvlLvl)
                return true;
            return collectedStars >= neededStars;
        }
    }
}