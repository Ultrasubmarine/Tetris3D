using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum GameState2 {
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

    [SerializeField, HideInInspector] List<bool> StateTable = new List<bool>();
    private GameState2 _currState;

    private int _countState;
    [SerializeField] bool setText;
    public Text UIText;


    void Start () {
        _countState = Enum.GetValues(typeof(GameState2)).Length;
        _currState = GameState2.NotActive;
        // SetState(GameState2.Empty);
    }

    public void ChangeState(int newState) {
        ChangeState((GameState2)newState);    
    }

    public bool ChangeState(GameState2 newState, bool broadcust = true) {
        
        if (StateTable[GetIndex(newState)]) {
            SetState(newState, broadcust);
            return true;
        }
        Debug.Log("Can'T change");
        return false;
    }

    private void SetState(GameState2 newState, bool broadcust = true) {
        _currState = newState;
        if(broadcust)
            Messenger.Broadcast(StateMachineKey + newState.ToString(), MessengerMode.REQUIRE_LISTENER);
        UIText.text = newState.ToString();
    }

    private int GetIndex( GameState2 newState ) {

        return (int)_currState * _countState + (int)newState;
    }
}
