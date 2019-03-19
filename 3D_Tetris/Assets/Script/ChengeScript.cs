using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChengeScript : MonoBehaviour {

    [SerializeField] UnityEvent HideScreen;
    [SerializeField] UnityEvent EndScreenChange;
    [SerializeField] Camera GameCamera;

    public void Hide()
    {
        HideScreen.Invoke();
        Debug.Log("Gid");
        GameCamera.GetComponent<GameCameraScript>().FirstAnimation();
    }

    public void EndChange()
    {       
        EndScreenChange.Invoke();
    }

  


}
