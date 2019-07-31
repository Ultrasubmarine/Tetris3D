using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Broadcaster : MonoBehaviour {

    [FormerlySerializedAs("BroadcastEventName")] [SerializeField] string _BroadcastEventName;

    public void Broadcast()
    {
        Messenger.Broadcast(_BroadcastEventName);
    }
}
