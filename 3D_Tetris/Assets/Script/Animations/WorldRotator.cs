using UnityEngine;

public class WorldRotator : MonoBehaviour
{
    [SerializeField] private Transform _rotationObject;

    [SerializeField] private float _speed;

    // Update is called once per frame
    void Update()
    {
        _rotationObject.Rotate(Vector3.up, Time.deltaTime * _speed);   
    }
}
