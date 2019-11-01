using UnityEngine;
using UnityEngine.Events;

namespace Helper.Patterns.Messenger
{
    public class Listener<T> : MonoBehaviour
    {
        [SerializeField] private T _EventName;
        [SerializeField] private UnityEvent _EventListener = new UnityEvent();

        private void Awake()
        {
            Messenger.AddListener(_EventName.ToString(), _EventListener.Invoke);
        }

        private void OnDestroy()
        {
            Messenger.RemoveListener(_EventName.ToString(), _EventListener.Invoke);
        }
    }
}