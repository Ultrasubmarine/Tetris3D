using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Listener : MonoBehaviour {

    [SerializeField] GameEvent _Eventname;
    [SerializeField] UnityEvent _EventListener = new UnityEvent();
    private void Awake()
    {
        Messenger.AddListener(_Eventname.ToString(), _EventListener.Invoke);
    }

    void OnDestroy()
    {
        Messenger.RemoveListener(_Eventname.ToString(), _EventListener.Invoke);
    }

}
