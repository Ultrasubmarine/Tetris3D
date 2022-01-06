using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script.GameLogic.Stars
{
    class StarInfo
    {
        public Block block;
        public float r;

        public StarInfo(Block block, float r)
        {
            this.block = block;
            this.r = r;
        }
    }

    public class StarsManager : MonoBehaviour
    {
        public bool collectStarLvl { get { return _collectStars; } }
        [SerializeField] private bool _collectStars;
        
        private List<Block> Stars;
        
        [SerializeField] private int _maxStarsAmount = 1;
        private List<Block> _applicants;

        private PlaneMatrix _matrix;

        public Action OnCreatedStar;

        [SerializeField] private Mesh _starMesh;

        public int collectedStars { get; private set; }
        [SerializeField] private int _neededStars = 5;
        public int neededStars => _neededStars;

        public Action OnUpdatedCollectingStars;

        [Header("Animation")]
        [SerializeField] private Transform _rotationStar;
        [SerializeField] private Transform _animationStar;
        [SerializeField] private List<Transform> _animationPathObjects;
        private List<Vector3> _animationPath;

        [SerializeField] private float _firstPointR = 3.5f;
        [SerializeField] private float _secondPointR = 2.5f;
        [SerializeField] private float _secondPointYP = 2.5f;
      //  private Path p;
      

      [SerializeField] private Material _starmaterial;
      [FormerlySerializedAs("_starSpeed")] [SerializeField] private float _starRotationSpeed = 5.0f;
      [SerializeField] private float _fallStarRotationSpeed = 5.0f;

      [SerializeField] private ParticleSystem _particles;
      private float z = 0;
      private Transform CameraTransform;

      private List<StarInfo> _RotationStars;
      private bool isParticle = false;
      
        private void Start()
        {
            _RotationStars = new List<StarInfo>();
            Stars = new List<Block>();
            _applicants = new List<Block>();
            _animationPath = new List<Vector3>();
            
            _matrix = RealizationBox.Instance.matrix;

            foreach (var go in _animationPathObjects)
            {
                _animationPath.Add(go.position);
            }
        }

        private void Update()
        {
            foreach (var s in _RotationStars)
            {
                s.block.Star.LookAt(Camera.main.transform);
                s.r += Time.deltaTime *_starRotationSpeed;
                if (s.r > 360.0f)
                {
                    s.r = 0.0f;
                }
                s.block.oreol.localRotation = Quaternion.Euler(0, 0, s.r);
               // s.block.myTransform.localRotation = Quaternion.Euler(0, s.r, 0);
            }
        }

        public bool CanCreateStar()
        {
            CameraTransform = RealizationBox.Instance.gameCamera.transform;
            if (Stars.Count >= _maxStarsAmount)
                return false;
            
            _applicants.Clear();
            for (int i = 0; i <= _matrix.height; i++)
            {
                if (Stars.Exists((Block b) => { return b._coordinates.y == i; }))
                    continue;

                AddApplicants(0, i, 0);
                AddApplicants(0, i, _matrix.wight-1);
                AddApplicants(_matrix.wight-1, i, 0);
                AddApplicants(_matrix.wight-1, i, _matrix.wight-1);
            }
            
            if (_applicants.Count > 0)
                return true;
            return false;
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
            var rndBlock = _applicants[Random.Range(0, _applicants.Count)];
            
            Stars.Add(rndBlock);
            rndBlock.OnCollected += CollectStar;
            CreateAnimation(rndBlock);
        }

        public void CreateAnimation(Block endPoint)
        {
            isParticle = false;
            _animationStar.gameObject.SetActive(true);
            _animationPath.Add(endPoint.transform.position);
            _animationPath[0] = new Vector3(endPoint.transform.position.x * _firstPointR, _animationPath[0].y, endPoint.transform.position.z * _firstPointR);
            _animationPath[1] = new Vector3(endPoint.transform.position.x * _secondPointR + 0.5f * _secondPointR  , endPoint.transform.position.y + _secondPointYP, endPoint.transform.position.z * _secondPointR + 0.5f * _secondPointR );

            
            _animationStar.position = _animationPath[0];
            _animationStar.LookAt(Camera.main.transform);
            _animationStar.DOPath(_animationPath.ToArray(), 2, PathType.CatmullRom, PathMode.TopDown2D).
                    OnUpdate(() =>
                    {
                        z += Time.deltaTime *_fallStarRotationSpeed;
                        if (z > 360.0f)
                            {
                                z = 0.0f;
                            }
                        _rotationStar.localRotation = Quaternion.Euler(0, 0, z);
                        _animationStar.LookAt(CameraTransform);

                        if (!isParticle && Vector3.Distance(_animationStar.transform.position, _animationPath[2]) < 1)
                        {
                            _particles.transform.position = endPoint.myTransform.position;
                            _particles.gameObject.SetActive(false);
                            _particles.gameObject.SetActive(true);

                            isParticle = true;
                        }
                    }).
              OnComplete(()=> 
              {
                  endPoint.TransformToStar(_starMesh, _starmaterial);
                  
                  _animationPath.RemoveAt(2);
                  _animationStar.DOKill();
                  _animationStar.gameObject.SetActive(false);
                  OnCreatedStar?.Invoke();
                  _RotationStars.Add(new StarInfo(endPoint, 0));
              });

            _animationStar.DOLocalRotate(Vector3.forward, 4).SetLoops(-1,LoopType.Incremental);
        }

        
        public void CollectStar(Block star)
        {
            star.OnCollected -= CollectStar;
            collectedStars++;
            Stars.Remove(star);

            StarInfo rStr;
            foreach (var s in _RotationStars)
            {
                if (s.block == star)
                {
                    rStr = s;
                    _RotationStars.Remove(rStr);
                    break;
                }
            }
            OnUpdatedCollectingStars?.Invoke();
        }

        public void Clear()
        {
            Stars.Clear();
            _applicants.Clear();
            collectedStars = 0;
            OnUpdatedCollectingStars?.Invoke();
        }

        public bool CheckWin()
        {
            if (!collectStarLvl)
                return true;
            return collectedStars >= neededStars;
        }
    }
}