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

    void Start() {
        PlaneGame = PlaneScript.Instance;
    }

    public void Play() {
        PlaneGame.StartGame();
    }

    public void Win() {
    }

    public void End() {
    }

    public void Restart() {
    }
}