using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameCamera : MonoBehaviour {
    
    [SerializeField] private Transform _MaxDistance;
    [SerializeField] private Transform _MinDistance;

    [SerializeField] private float _MaxSize;
    [SerializeField] private float _MinSize;

    [SerializeField] private Transform _MaxLookAt;
    [SerializeField] private Transform _MinLookAt;

    [SerializeField] private float _TimeStabilization;

    [SerializeField] private Transform _ObjectLook;

    private Transform _myTransform;

    private int _currentHeight = 0;
    private Camera _camera;

    private Vector3 _offset; // положение между камерой и площадкой
    private float _rotY;
    
    [Space(15)]
    [Header("First Animation")]
    [SerializeField] private float _Time = 1;
    [SerializeField] private float _FirstOrthographicSize = 15;

    private void Awake() {
        Messenger<int, int>.AddListener(GameEvent.CURRENT_HEIGHT.ToString(), CheckStabilization);
        _myTransform = transform;
    }

    // Use this for initialization
    private void Start() {
        _offset = Vector3.zero - transform.position;
        _myTransform.LookAt(_ObjectLook.transform.position);

        _camera = GetComponent<Camera>();
        _myTransform.LookAt(_ObjectLook.position);
    }

    public void CheckStabilization(int limit, int height) {
        if (_currentHeight == height)
            return;

        _currentHeight = height;
        StartCoroutine(ChangeDistance(limit, height));
    }

    public IEnumerator ChangeDistance(int limit, int current) {
        Vector3 needPosition = Vector3.Lerp(_MinDistance.position, _MaxDistance.position, current / (float) limit);

        float t = 0;
      
        _offset = Vector3.zero - needPosition;
        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);

        Vector3 startP = _myTransform.position;
        Vector3 finishP = Vector3.zero - (rotation * _offset);

        float startS = _camera.orthographicSize;
        float finishS = Mathf.Lerp(_MinSize, _MaxSize, current / (float) limit);

        Vector3 startLookAt = _ObjectLook.position;
        Vector3 finishLookAt = Vector3.Lerp(_MinLookAt.position, _MaxLookAt.position,current / (float) limit);

        while (t < _TimeStabilization) {
            _myTransform.position = Vector3.Lerp(startP, finishP, t / _TimeStabilization);
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, t / _TimeStabilization);

            _ObjectLook.position = Vector3.Lerp(startLookAt, finishLookAt, t / _TimeStabilization);
            _myTransform.LookAt(_ObjectLook.position);

            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        _myTransform.position = finishP;
        _camera.orthographicSize = finishS;
        _ObjectLook.transform.position = finishLookAt;
        _myTransform.LookAt(_ObjectLook.position);
    }

    public void FirstAnimation() {
        StartCoroutine(ChangeCameraSize());
    }

    public IEnumerator ChangeCameraSize() {
        float timer = 0;

        float startS = _FirstOrthographicSize;
        float finishS = _MinSize;
        while (timer < _Time) {
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, timer / _Time);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Messenger.Broadcast(GameEvent.PLAY_GAME.ToString());
    }
}