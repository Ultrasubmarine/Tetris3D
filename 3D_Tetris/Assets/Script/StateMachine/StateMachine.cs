using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum EMachineState {
    NotActive,

    Empty,
    NewElement,

    Turn,
    Move,
    Merge,

    Collection,
    DropAllElements,

    Win,
    End,
}

[ExecuteInEditMode]
public class StateMachine : MonoBehaviour {

    public const string StateMachineKey = "STATE MACHINE <GAME> ";

    [FormerlySerializedAs("StateTable")] [SerializeField, HideInInspector] List<bool> _StateTable = new List<bool>();
    [SerializeField] private EMachineState _currState;
    private int _countState;

    public EMachineState State { get { return _currState; } }

    void Start () {
        _countState = Enum.GetValues(typeof(EMachineState)).Length;
        _currState = EMachineState.NotActive;
    }

    public void ChangeState(int newState) {
        ChangeState((EMachineState)newState);    
    }

    public bool ChangeState(EMachineState newState, bool broadcast = true) {
        
        if (_StateTable[GetIndex(newState)]) {
            SetState(newState, broadcast);
            return true;
        }
        Debug.Log("Can'T change/ current:" + State.ToString() + " you want: " + newState.ToString());
        return false;
    }

    private void SetState(EMachineState newState, bool broadcast = true) {
        _currState = newState;
        if(broadcast)
            Messenger.Broadcast(StateMachineKey + newState.ToString(), MessengerMode.REQUIRE_LISTENER);
    }

    private int GetIndex( EMachineState newState ) {
        
        return (int)_currState * _countState + (int)newState;
    }
}
