﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    [SerializeField] int ScoreForWin;
    [SerializeField] int CurrentScore;

    [SerializeField] Text ScoreText;

	// Use this for initialization
	void Start () {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER, ScoreeIncrement);
        ScoreText.text = CurrentScore.ToString() + "/" + ScoreForWin.ToString() + " m";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ScoreeIncrement(int layer)
    {
        CurrentScore += 9;
        ScoreText.text = CurrentScore.ToString() + "/" + ScoreForWin.ToString() + " m";
        CheckWin();
    }

    public void CheckWin()
    {
        if(CurrentScore >= ScoreForWin)
        {
            Messenger.Broadcast(GameEvent.WIN_GAME);
            Debug.Log("WIN");
        }
    }
}