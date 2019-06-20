using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameCameraState {
    rotetaState,
    gameState,
    stabilization,
}

public class GameCameraScript : MonoBehaviour {
    [SerializeField] private Transform MaxDistance;
    [SerializeField] private Transform MinDistance;

    [SerializeField] private float MaxSize;
    [SerializeField] private float MinSize;

    [SerializeField] private Transform MaxLookAt;
    [SerializeField] private Transform MinLookAt;

    [SerializeField] private float _TimeStabisization;

    [SerializeField] private GameObject ObjectLook;

    private Transform _myTransform;

    private int _current = 0;
    private Camera _camera;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры

    [SerializeField] StateMachine _StateMachine;

    private void Awake() {
        Messenger<int, int>.AddListener(GameEvent.CURRENT_HEIGHT, CheckStabilization);
        _myTransform = transform;
    }

    // Use this for initialization
    private void Start() {
        _offset = Vector3.zero - transform.position; // сохраняем расстояние между камерой и полем
        _myTransform.LookAt(ObjectLook.transform.position);

        _camera = GetComponent<Camera>();
        transform.LookAt(ObjectLook.transform.position);
    }

    public void CheckStabilization(int Limit, int Current) {
        if (_current == Current)
            return;

        _current = Current;
        StartCoroutine(ChangeDistance(Limit, Current));
    }

    public IEnumerator ChangeDistance(int Limit, int Current) {
        Vector3 needPosition = Vector3.Lerp(MinDistance.position, MaxDistance.position, Current / (float) Limit);

        float t = 0;
        // для подсчета конечного поворота
        _offset = Vector3.zero - needPosition; // сохраняем НОВОЕ расстояние между камерой и полем
        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);

        Vector3 startP = _myTransform.position;
        Vector3 finishP = Vector3.zero - (rotation * _offset);

        float startS = _camera.orthographicSize;
        float finishS = Mathf.Lerp(MinSize, MaxSize, Current / (float) Limit); //Mathf.Sqrt( Current / (float)Limit) );

        Vector3 startLookAt = ObjectLook.transform.position;
        Vector3 finishLookAt =
            Vector3.Lerp(MinLookAt.position, MaxLookAt.position,
                Current / (float) Limit); // Mathf.Sqrt (Current / (float)Limit));

        while (t < _TimeStabisization) {
            _myTransform.position = Vector3.Lerp(startP, finishP, t / _TimeStabisization);
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, t / _TimeStabisization);

            // изменение элемента на который смотрим
            ObjectLook.transform.position = Vector3.Lerp(startLookAt, finishLookAt, t / _TimeStabisization);
            _myTransform.LookAt(ObjectLook.transform.position);

            yield return null;
            t += Time.deltaTime;
        }

        // конечное положение 
        _myTransform.position = finishP;
        _camera.orthographicSize = finishS;
        ObjectLook.transform.position = finishLookAt;
        _myTransform.LookAt(ObjectLook.transform.position);
    }

    public void FirstAnimation() {
        StartCoroutine(ChangeCameraSize());
    }

    public IEnumerator ChangeCameraSize() {
        //TODO надо их в инспектор вынести или конвертировать в константы?
        /////
        float timer = 0;
        float duration = 1.0f;

        float startS = 15;
        float finishS = 8.1f;
        /////
        while (timer < duration) {
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        Messenger.Broadcast(GameEvent.PLAY_GAME);
    }
}