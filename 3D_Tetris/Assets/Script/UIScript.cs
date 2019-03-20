using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour {
    static bool _musicState = true;

    public void UI_StartGame() {
        Messenger.Broadcast(GameEvent.UI_PLAY);
    }

    public void UI_PauseGame(bool change) {
        Time.timeScale = change ? 0 : 1;
    }

    public void UI_MusicChange() {
    }
}