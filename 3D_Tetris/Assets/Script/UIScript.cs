using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour {

    static bool _musicState = true;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UI_StartGame()
    {
        Messenger.Broadcast(GameEvent.UI_PLAY);
    }

    public void UI_PauseGame( bool chenge)
    {
        if (chenge == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void UI_MusicChenge()
    {

    }

}
