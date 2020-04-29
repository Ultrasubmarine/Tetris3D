using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
    public event Action onFirstAnimationEnd;
    
    [SerializeField] private Transform _maxDistance;
    [SerializeField] private Transform _minDistance;

    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;

    [SerializeField] private Transform _maxLookAt;
    [SerializeField] private Transform _minLookAt;

    [SerializeField] private float _timeStabilization;

    [SerializeField] private Transform _objectLook;
    
    private Camera _camera;
   
    private Transform _myTransform;
    
    private HeightHandler _heightHandler;

    private int _currentHeight = 0;

    [Space(15)] [Header("First Animation")] [SerializeField]
    private float _Time = 1;

    [SerializeField] private float _FirstOrthographicSize = 15;

    
    
    public void FirstAnimation()
    {
        _camera.DOOrthoSize(_minSize, _Time).From(_FirstOrthographicSize)
            .OnComplete(() => onFirstAnimationEnd?.Invoke());
    }
    
    public void SetStabilization()
    {
        CheckStabilization(_heightHandler.limitHeight, _heightHandler.currentHeight);
    }
    
    private void Awake()
    {
        _myTransform = transform;
        _camera = GetComponent<Camera>();
    }

    // Use this for initialization
    private void Start()
    {
        _myTransform.LookAt(_objectLook.transform.position);
        _myTransform.LookAt(_objectLook.position);

        _heightHandler = RealizationBox.Instance.haightHandler;
    }

    private void CheckStabilization(int limit, int height)
    {
        if (_currentHeight == height)
            return;

        _currentHeight = height;
        
        
        var finishP = Vector3.Lerp(_minDistance.position, _maxDistance.position, height / (float) limit);
        var finishS = Mathf.Lerp(_minSize, _maxSize, height / (float) limit);
        var finishLookAt = Vector3.Lerp(_minLookAt.position, _maxLookAt.position, height / (float) limit);

        _myTransform.DOMove(finishP, _timeStabilization);
        _camera.DOOrthoSize(finishS, _timeStabilization);
        _objectLook.DOMove(finishLookAt, _timeStabilization).OnUpdate(() => _myTransform.LookAt(_objectLook.position));
    }
}