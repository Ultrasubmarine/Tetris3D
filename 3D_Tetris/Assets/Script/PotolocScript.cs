using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotolocScript : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("В НАС ПОПАЛИ");
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        //var mr = temp.GetComponentInChildren<MeshRenderer>(); // для дочерних
        meshRenderer.enabled = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        //var mr = temp.GetComponentInChildren<MeshRenderer>(); // для дочерних
        meshRenderer.enabled = false;
    }
}
