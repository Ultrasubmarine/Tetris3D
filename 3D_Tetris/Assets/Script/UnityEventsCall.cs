using UnityEngine;
using UnityEngine.Events;

public class UnityEventsCall : MonoBehaviour
{
    [SerializeField] private UnityEvent _events;

    public void Call()
    {
        _events.Invoke();
    }
}
