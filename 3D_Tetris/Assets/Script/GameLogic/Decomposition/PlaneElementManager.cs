using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneElementManager : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AfterDestroyLayer() {

        DestroyEmptyElement();
        CutElement();

        StartCoroutine(DropAfterDestroy());

    }

    private IEnumerator DropAfterDestroy() {
        yield return null;
    }
    public void CutElement() {
        
    }

    public void DestroyEmptyElement() {

    }

}
