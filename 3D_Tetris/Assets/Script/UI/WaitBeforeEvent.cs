using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaitBeforeEvent : MonoBehaviour
{
    [SerializeField] private float _Timer;
    [SerializeField] private UnityEvent _InvokeAfterTime;

    private void Start()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(_Timer);
        _InvokeAfterTime.Invoke();
    }
}