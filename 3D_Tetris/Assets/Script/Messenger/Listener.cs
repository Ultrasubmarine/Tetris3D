using UnityEngine;
using UnityEngine.Events;

namespace Helper.Patterns.Messenger
{
    public class Listener<T> : MonoBehaviour {

        [SerializeField] T _EventName;
        [SerializeField] UnityEvent _EventListener = new UnityEvent();
        private void Awake()
        {
            Messenger.AddListener(_EventName.ToString(), _EventListener.Invoke);
        }

        void OnDestroy()
        {
            Messenger.RemoveListener(_EventName.ToString(), _EventListener.Invoke);
        }
    }
}
