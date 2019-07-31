using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Broadcaster : MonoBehaviour {
    [SerializeField] GameEvent _BroadcastEventName;

    public void Broadcast()
    {
        Messenger.Broadcast(_BroadcastEventName.ToString());
    }
}
