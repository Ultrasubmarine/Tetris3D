using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

enum GameState {
    Play,
    Replay,
    Win,
    End,
}

public class GameManager : MonoBehaviour {
    
//    [SerializeField] StateMachine _Machine;
    [SerializeField] GameObject _Controller;

    public void StartGame() {
        _Controller.SetActive(true);
//        _Machine.ChangeState(EMachineState.Empty);
    }

    public void ReplayGame() {
//        _Machine.ChangeState(EMachineState.NotActive);
    }

}