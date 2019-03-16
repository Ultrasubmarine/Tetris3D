using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    play,
    win,
    end,
}
public class GameManager : MonoBehaviour {

    PlaneScript PlaneGame;
	// Use this for initialization
	void Start () {
        PlaneGame = PlaneScript.Instanse;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play()
    {
        PlaneGame.StartGame();
    }

    public void Win()
    {
        
    }

    public void End()
    {

    }

    public void Restart()
    {

    }
}
