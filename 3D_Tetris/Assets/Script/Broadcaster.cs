using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcaster : MonoBehaviour {

    [SerializeField] string BroadcastEventName;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Broadcast()
    {
        Messenger.Broadcast(BroadcastEventName);
    }
}
