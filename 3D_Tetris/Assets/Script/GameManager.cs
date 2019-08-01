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
    
    [FormerlySerializedAs("machine")] [SerializeField] StateMachine _machine;
    public void StartGame() {

        _machine.ChangeState(EMachineState.Empty);
    }

    private void RepleyGame() {
        StartGame();
    }


    public void End() {
    }

    public void Restart() {
    }
}