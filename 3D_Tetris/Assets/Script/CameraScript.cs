using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState {
    rotetaState,
    gameState,
}

public class CameraScript : MonoBehaviour {
    [SerializeField] private float _DegreesPerSecond = 5;

    void deletewindiws(int x) {
    }

    [SerializeField] private turn _Direction;

    /*
     TODO Тут надо хранить Transform объектов!
     Ппервое обращение к GameObject.transform вызывает GameObject.GetComponent<Transform>, 
     которое (не знаю точно с какой версии, но знаю, что не идеально - почитем) юнити самостоятельно уже кэширует
    */
    //
    [SerializeField] private Transform _ObjectLook;
    [SerializeField] private Transform _OnlyLookObjectLook;

    [SerializeField] private Transform _ObjectCamera2;
    //
    private Transform _myTransform;

    private float _rotY = 0;
    private Vector3 _offset; // начальное положение между камерой и площадкой
    private CameraState _myState = CameraState.rotetaState;


    [SerializeField] private float _Height = 30;
    [SerializeField] private float _TimeLookUp;
    [SerializeField] private float _TimeLookDown;
    
    [SerializeField] private Camera _PlayerCamera;
    Camera _camera;

    private void Awake() {
        _camera = GetComponent<Camera>();
        _myTransform = GetComponent<Transform>();

        Messenger.AddListener(GameEvent.UI_PLAY, StartGame);
    }

    private void Start() {
        _offset = _ObjectLook.position - _myTransform.position; // сохраняем расстояние между камерой и полем
        StartCoroutine(RotationCoroutine());
    }

    private void StartGame() {
        _myState = CameraState.gameState;

        //ya NE typaya, I am very smart!

        StartCoroutine(StartLookTop());
    }

    private IEnumerator RotationCoroutine() {
        int angle = _Direction == turn.left ? 90 : -90;

        while (_myState == CameraState.rotetaState) {
            Quaternion rotationStart = Quaternion.Euler(0, _rotY, 0);
            Quaternion rotationEnd = Quaternion.Euler(0, _rotY + angle, 0);

            float timeRotate = Mathf.Abs(angle / _DegreesPerSecond);
            float countTime = 0;
            while (countTime < timeRotate && _myState == CameraState.rotetaState) {
                if (countTime + Time.deltaTime < timeRotate)
                    countTime += Time.deltaTime;
                else
                    countTime = timeRotate;

                _rotY += _DegreesPerSecond * Time.deltaTime;

                _myTransform.position = _ObjectLook.position -
                                     Quaternion.LerpUnclamped(rotationStart, rotationEnd, countTime / timeRotate) *
                                     _offset;
                _myTransform.LookAt(_OnlyLookObjectLook.position);

                yield return null;
            }

            if (_rotY == 360 || _rotY == -360)
                _rotY = 0;
        }
    }

    private IEnumerator StartLookTop() {
        Vector3 startPosition = _OnlyLookObjectLook.position;
        Vector3 finalPosition = startPosition + new Vector3(0.0f, _Height, 0.0f);

        // смотрим вверх

        float time = 0;
        while (time < _TimeLookUp) {
            if (time + Time.deltaTime < _TimeLookUp)
                time += Time.deltaTime;
            else
                time = _TimeLookUp;

            _OnlyLookObjectLook.position = Vector3.Lerp(startPosition, finalPosition, time / _TimeLookUp);
            transform.LookAt(_OnlyLookObjectLook.position);

            yield return null;
        }

        _ObjectCamera2.LookAt(_OnlyLookObjectLook.position);
        _camera.enabled = false;
        // опускаемся вниз ( уже другой камерой)

        time = 0;
        while (time < _TimeLookDown) {
            if (time + Time.deltaTime < _TimeLookDown)
                time += Time.deltaTime;
            else
                time = _TimeLookDown;

            _OnlyLookObjectLook.position = Vector3.Lerp(finalPosition, startPosition, time / _TimeLookDown);
            _ObjectCamera2.LookAt(_OnlyLookObjectLook.position);

            // Debug.Log(" return OBJECT/ countTime=" + countTime + " _TimeLookUp = " + _TimeLookDown);
            yield return null;
        }

        Messenger.Broadcast(GameEvent.PLAY_GAME);
    }
}