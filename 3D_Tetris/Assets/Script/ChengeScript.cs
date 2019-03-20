using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChengeScript : MonoBehaviour {
    [SerializeField] UnityEvent HideScreen;
    [SerializeField] UnityEvent EndScreenChange;
    [SerializeField] Camera GameCamera;
    GameCameraScript _gameCameraScript;
    void Awake() {
        _gameCameraScript = GetComponent<GameCameraScript>();
    }

    public void Hide() {
        HideScreen.Invoke();
        Debug.Log("Gid");
//        GameCamera.GetComponent<GameCameraScript>().FirstAnimation();
        _gameCameraScript.FirstAnimation();
    }

    public void EndChange() {
        EndScreenChange.Invoke();
    }
}