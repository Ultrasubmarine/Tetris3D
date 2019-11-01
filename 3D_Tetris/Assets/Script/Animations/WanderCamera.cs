using System.Collections;
using UnityEngine;

public enum CameraState
{
    RotateState,
    GameState,
}

public class WanderCamera : MonoBehaviour
{
    [SerializeField] private float _DegreesPerSecond = 5;
    [SerializeField] private turn _Direction;
    [SerializeField] private Transform _ObjectLook;
    private Vector3 _objLookPosition;
    private Transform _myTransform;

    private float _rotY = 0;
    private Vector3 _offset; // положение между камерой и площадкой
    private CameraState _myState = CameraState.RotateState;

    private void Awake()
    {
        _myTransform = GetComponent<Transform>();
//        Messenger.AddListener(GameEvent.UI_PLAY.ToString(), StartGame);
    }

    private void Start()
    {
        _objLookPosition = _ObjectLook.position;
        _offset = _objLookPosition - _myTransform.position; // сохраняем расстояние между камерой и полем
        StartCoroutine(RotationCoroutine());
    }

    private void StartGame()
    {
        _myState = CameraState.GameState;
        Debug.Log("End wander");
        gameObject.SetActive(false);
    }

    private IEnumerator RotationCoroutine()
    {
        var angle = _Direction == turn.left ? 90 : -90;

        while (_myState == CameraState.RotateState)
        {
            var rotationStart = Quaternion.Euler(0, _rotY, 0);
            var rotationEnd = Quaternion.Euler(0, _rotY + angle, 0);

            var timeRotate = Mathf.Abs(angle / _DegreesPerSecond);
            float countTime = 0;
            while (countTime < timeRotate && _myState == CameraState.RotateState)
            {
                if (countTime + Time.deltaTime < timeRotate)
                    countTime += Time.deltaTime;
                else
                    countTime = timeRotate;

                _rotY += _DegreesPerSecond * Time.deltaTime;

                _myTransform.position = _objLookPosition -
                                        Quaternion.LerpUnclamped(rotationStart, rotationEnd, countTime / timeRotate) *
                                        _offset;
                _myTransform.LookAt(_objLookPosition);

                yield return new WaitForEndOfFrame();
            }

            if (_rotY >= 360 || _rotY <= -360)
                _rotY = 0;
        }
    }
}