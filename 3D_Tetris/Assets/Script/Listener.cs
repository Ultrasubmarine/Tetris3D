using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Listener : MonoBehaviour {

    [SerializeField] string Eventname;
    [SerializeField] UnityEvent EventListener = new UnityEvent();

    private void Start()
    {
        Messenger.AddListener(Eventname, EventListener.Invoke);
    }

    private void OnEnable()
    {
        Messenger.RemoveListener(Eventname, EventListener.Invoke);
    }

}
