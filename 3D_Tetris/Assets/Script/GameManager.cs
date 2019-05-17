using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState {
    Play,
    Win,
    End
}

public class GameManager : MonoBehaviour {
    PlaneScript PlaneGame;
    [SerializeField] StateMachine machine; 

    void Start() {
        PlaneGame = PlaneScript.Instance;
    }

    public void StartGame() {

        Debug.Log("666 START 666");
        machine.ChangeState(GameState2.Empty);
    }

    private void RepleyGame() {
        //_ElementManager.DestroyAllElements();
        //_HeightHandler.CheckHeight();
        StartGame();
        //  Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, 0);
    }


    public void End() {
    }

    public void Restart() {
    }
}