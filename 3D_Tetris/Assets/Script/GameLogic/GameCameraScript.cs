using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameCameraState
{
    rotetaState,
    gameState,
    stabilization,
}

public class GameCameraScript : MonoBehaviour
{
    [SerializeField] private Transform MaxDistance;
    [SerializeField] private Transform MinDistance;

    [SerializeField] private float MaxSize;
    [SerializeField] private float MinSize;

    [SerializeField] private Transform MaxLookAt;
    [SerializeField] private Transform MinLookAt;

    [SerializeField] private float _TimeStabisization;

    [SerializeField] private GameObject ObjectLook;

    private int _current = 0;
    private Camera _camera;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY;  // поворот камеры

    private void Awake()
    {
        Messenger<int, int>.AddListener(GameEvent.CURRENT_HEIGHT, CheckStabilization);
    }

    // Use this for initialization
    private void Start()
    {
        _offset = Vector3.zero - transform.position; // сохраняем расстояние между камерой и полем
        transform.LookAt(ObjectLook.transform.position);

        _camera = GetComponent<Camera>();
    }

    public IEnumerator TurnCamera(turn direction, float time)
    {
        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        // начальный и конечный поворот
        Quaternion rotationStart = Quaternion.Euler(0, _rotY, 0);
        _rotY += angle;
        Quaternion rotationEnd = Quaternion.Euler(0, _rotY, 0);

        float countTime = 0;

        while (countTime < time)
        {
            if ((countTime + Time.deltaTime) < time)
                countTime += Time.deltaTime;
            else
                countTime = time;

            transform.position = Vector3.zero - (Quaternion.LerpUnclamped(rotationStart, rotationEnd, (countTime / time)) * _offset);
            transform.LookAt(ObjectLook.transform.position);

            yield return null;
        }

        if (_rotY == 360 || _rotY == -360)
            _rotY = 0;

        yield break;
    }

    public void CheckStabilization(int Limit, int Current)
    {
        if (_current == Current)
            return;

        _current = Current;
        StartCoroutine(ChangeDistance(Limit, Current));
    }

    public IEnumerator ChangeDistance(int Limit, int Current)
    {
        Vector3 needPosition = Vector3.Lerp(MinDistance.position, MaxDistance.position, Current / (float)Limit);

        float t = 0;

        // для подсчета конечного поворота
        _offset = Vector3.zero - needPosition; // сохраняем НОВОЕ расстояние между камерой и полем
        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);

        Vector3 startP = transform.position;
        Vector3 finishP = Vector3.zero - (rotation * _offset);

        float startS = _camera.orthographicSize;
        float finishS = Mathf.Lerp(MinSize, MaxSize, Current / (float)Limit); //Mathf.Sqrt( Current / (float)Limit) );

        Vector3 startLookAt = ObjectLook.transform.position;
        Vector3 finishLookAt = Vector3.Lerp(MinLookAt.position, MaxLookAt.position, Current / (float)Limit);// Mathf.Sqrt (Current / (float)Limit));

        while (t < _TimeStabisization)
        {        
            this.transform.position = Vector3.Lerp(startP, finishP,  t / _TimeStabisization);
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, t / _TimeStabisization);

            // изменение элемента на который смотрим
            ObjectLook.transform.position = Vector3.Lerp(startLookAt, finishLookAt, t / _TimeStabisization) ;
            transform.LookAt(ObjectLook.transform.position);

            yield return null;
            t += Time.deltaTime;
        }

        // конечное положение 
        this.transform.position = finishP;
        _camera.orthographicSize =  finishS;
        ObjectLook.transform.position = finishLookAt;
        transform.LookAt(ObjectLook.transform.position);


        yield break;
    }

    public void FirstAnimation()
    {
        StartCoroutine(ChengeCameraSize());
    }
    public IEnumerator ChengeCameraSize()
    {
        float t = 0;
        float T = 1.0f;

        float startS = 15;
        float finishS = 8.1f;
        
        while (t < T)
        {
            _camera.orthographicSize = Mathf.Lerp(startS, finishS, t / T);
            t += Time.deltaTime;
            yield return null;
        }

         Messenger.Broadcast(GameEvent.PLAY_GAME);
        yield break;
    }
}