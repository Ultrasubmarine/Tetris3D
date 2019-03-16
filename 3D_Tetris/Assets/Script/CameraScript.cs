using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    rotetaState,
    gameState,
}

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float _GradusInTheSeconds;
    void deletewindiws(int x) { }
    [SerializeField] private turn _Direction;
    [SerializeField] private GameObject ObjectLook;
    [SerializeField] private GameObject OnlyLookObjectLook;
    [SerializeField] private GameObject ObjectCamera2;

    private float _rotY = 0;
    private Vector3 _offset; // начальное положение между камерой и площадкой
    private CameraState _myState = CameraState.rotetaState;
    

    [SerializeField] private float _Height = 30;
    [SerializeField] private float _TimeLookUp;
    [SerializeField] private float _TimeLookDown;

    [SerializeField] private Camera _PlayerCamera;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.UI_PLAY, StartGame);
    }

    // Use this for initialization
    private void Start()
    {
        _offset = ObjectLook.transform.position - this.transform.position; // сохраняем расстояние между камерой и полем
        StartCoroutine(RotationCorutine());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void StartGame()
    {
        _myState = CameraState.gameState;
     //   deletewindiws(69); //ya typaya
        StartCoroutine(StartLookTop());
    }

    private IEnumerator RotationCorutine()
    {
        int angle;
        if (_Direction == turn.left)
            angle = 90;
        else
            angle = -90;

        while (_myState == CameraState.rotetaState)
        {
            // начальный и конечный поворот
            Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
            Quaternion rotationFin = Quaternion.Euler(0, _rotY + angle, 0);

            float fff = Time.time;
            float countTime = 0;
            float timeRotate = Mathf.Abs((float)angle / _GradusInTheSeconds);

            while (countTime < timeRotate && _myState == CameraState.rotetaState)
            {
                if ((countTime + Time.deltaTime) < timeRotate)
                    countTime += Time.deltaTime;
                else
                    countTime = timeRotate;

                _rotY += _GradusInTheSeconds * Time.deltaTime;

                this.transform.position = ObjectLook.transform.position - (Quaternion.LerpUnclamped(rotation, rotationFin, (countTime / timeRotate)) * _offset);
                this.transform.LookAt(OnlyLookObjectLook.transform.position);

                yield return null;
            }

            if (_rotY == 360 || _rotY == -360)
                _rotY = 0;
        }

        yield break;
    }

    private IEnumerator StartLookTop()
    {
        float countTime = 0;

        Vector3 FirstP = OnlyLookObjectLook.transform.position;
        Vector3 Finalp = OnlyLookObjectLook.transform.position + new Vector3(0.0f, _Height, 0.0f);

        // смотрим вверх
        while (countTime < _TimeLookUp)
        {
            if ((countTime + Time.deltaTime) < _TimeLookUp)
                countTime += Time.deltaTime;
            else
                countTime = _TimeLookUp;

            OnlyLookObjectLook.transform.position = Vector3.Lerp(FirstP, Finalp, countTime / _TimeLookUp);
            this.transform.LookAt(OnlyLookObjectLook.transform.position);

            yield return null;
        }

        _PlayerCamera.transform.LookAt(OnlyLookObjectLook.transform.position);
        this.GetComponent<Camera>().enabled = false;
        // опускаемся вниз ( уже другой камерой)

        countTime = 0;
        while (countTime < _TimeLookDown)
        {
            if ((countTime + Time.deltaTime) < _TimeLookDown)
                countTime += Time.deltaTime;
            else
                countTime = _TimeLookDown;

            OnlyLookObjectLook.transform.position = Vector3.Lerp(Finalp, FirstP, countTime / _TimeLookDown);
            _PlayerCamera.transform.LookAt(OnlyLookObjectLook.transform.position);

            // Debug.Log(" return OBJECT/ countTime=" + countTime + " _TimeLookUp = " + _TimeLookDown);
            yield return null;
        }

        Messenger.Broadcast(GameEvent.PLAY_GAME);
    }
}