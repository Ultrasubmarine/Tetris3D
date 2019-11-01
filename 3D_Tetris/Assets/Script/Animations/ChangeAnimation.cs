using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeAnimation : MonoBehaviour
{
    [SerializeField] private UnityEvent HideScreen;
    [SerializeField] private UnityEvent EndScreenChange;
    [SerializeField] private Camera GameCamera;
    private GameCamera _gameCamera;

    private void Awake()
    {
        _gameCamera = GameCamera.GetComponent<GameCamera>();
    }

    public void Hide()
    {
        HideScreen.Invoke();
        _gameCamera.FirstAnimation();
    }

    public void EndChange()
    {
        EndScreenChange.Invoke();
    }
}