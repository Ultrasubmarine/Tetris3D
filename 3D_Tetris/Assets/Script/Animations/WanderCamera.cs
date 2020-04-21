using System.Collections;
using UnityEngine;

public class WanderCamera : MonoBehaviour
{
    [SerializeField] private float _degreesPerSecond = 5;
    
    [SerializeField] private turn _direction;
    
    [SerializeField] private Transform _objectLook;
    
    private Vector3 _objLookPosition;
    
    private Transform _myTransform;

    private float _rotY = 0;
    
    private Vector3 _offset; // положение между камерой и площадкой

    private string _coroutinesName;

    
    public void StartAnimation()
    {
        gameObject.SetActive(true);
        StartCoroutine(RotationCoroutine());
    }

    public void StopAnimation()
    {
        StopCoroutine(_coroutinesName);
        gameObject.SetActive(false);
    }
    
    private void Awake()
    {
        _myTransform = GetComponent<Transform>();
        _coroutinesName = nameof(RotationCoroutine);
    }

    private void Start()
    {
        _objLookPosition = _objectLook.position;
        _offset = _objLookPosition - _myTransform.position; // сохраняем расстояние между камерой и полем
        StartAnimation();
    }

    private IEnumerator RotationCoroutine()
    {
        var angle = _direction == turn.left ? 90 : -90;

        while(true)
        {
            var rotationStart = Quaternion.Euler(0, _rotY, 0);
            var rotationEnd = Quaternion.Euler(0, _rotY + angle, 0);

            var timeRotate = Mathf.Abs(angle / _degreesPerSecond);
            float countTime = 0;
            while (countTime < timeRotate)
            {
                if (countTime + Time.deltaTime < timeRotate)
                    countTime += Time.deltaTime;
                else
                    countTime = timeRotate;

                _rotY += _degreesPerSecond * Time.deltaTime;

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