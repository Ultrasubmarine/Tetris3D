using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeAnimation : MonoBehaviour {
    [SerializeField] UnityEvent HideScreen;
    [SerializeField] UnityEvent EndScreenChange;
    [SerializeField] Camera GameCamera;
    GameCamera _gameCamera;
    void Awake() {
        _gameCamera = GameCamera.GetComponent<GameCamera>();
    }

    public void Hide() {
        HideScreen.Invoke();
//        GameCamera.GetComponent<GameCameraScript>().FirstAnimation();
        _gameCamera.FirstAnimation();
    }

    public void EndChange() {
        EndScreenChange.Invoke();
    }
}