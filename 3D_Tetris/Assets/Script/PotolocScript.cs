using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotolocScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("В НАС ПОПАЛИ");
        var mr = this.gameObject.GetComponent<MeshRenderer>();
        //var mr = temp.GetComponentInChildren<MeshRenderer>(); // для дочерних
        mr.enabled = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        var mr = this.gameObject.GetComponent<MeshRenderer>();
        //var mr = temp.GetComponentInChildren<MeshRenderer>(); // для дочерних
        mr.enabled = false;
    }
}
