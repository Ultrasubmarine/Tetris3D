using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

enum GameState {
    Play,
    Win,
    End
}

public class GameManager : MonoBehaviour {
    
    [SerializeField] StateMachine _Machine;
    [SerializeField] GameObject _Controller;
    public void StartGame() {

        _Controller.SetActive( true );
        _Machine.ChangeState(EMachineState.Empty);
    }

    private void RepleyGame() {
        StartGame();
    }

    public void End() {
        _Controller.SetActive(false);
    }

    public void Restart() {
    }
}