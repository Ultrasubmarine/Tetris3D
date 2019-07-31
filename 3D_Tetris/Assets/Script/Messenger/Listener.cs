using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Listener : MonoBehaviour {

    [FormerlySerializedAs("Eventname")] [SerializeField] string _Eventname;
    [FormerlySerializedAs("EventListener")] [SerializeField] UnityEvent _EventListener = new UnityEvent();
    private void Awake()
    {
        Messenger.AddListener(_Eventname, _EventListener.Invoke);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(_Eventname, _EventListener.Invoke);
    }

}
