using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// you must bind EventInvoke with you functions, what you whant call

// if you want chenge call in Inspector Unity. 
[Serializable]
public class ListenerFunc2 : UnityEvent<int> { }

[Serializable]
public class Listener <T> : MonoBehaviour{

    public string EventName;
    public Action<T> CallDelegate  ;

    private void Start()    {
        Debug.Log("I Lissten " + EventName + "tttt");
        Messenger<T>.AddListener(EventName, CallDelegate);
    }

    private void OnEnable() {
    //    Messenger<T>.RemoveListener(EventName, CallDelegate);
    }

}

public class Listener<T, M> : MonoBehaviour
{

    [SerializeField] string EventName;
    Action<T,M> CallDelegate;

    private void Start()
    {
        Messenger<T,M>.AddListener(EventName, CallDelegate.Invoke);
    }

    private void OnEnable()
    {
        Messenger<T,M>.RemoveListener(EventName, CallDelegate.Invoke);
    }

}
