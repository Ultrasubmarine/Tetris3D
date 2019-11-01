using UnityEngine;

namespace Helper.Patterns.Messenger
{
    public class Broadcaster<T> : MonoBehaviour
    {
        [SerializeField] private T _BroadcastEventName;

        public void Broadcast()
        {
            Messenger.Broadcast(_BroadcastEventName.ToString());
        }
    }
}