using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState2 {
    NotActive,

    Empty,
    NewElement,

    Turn,
    Move,
    Merge,

    Collection,

    Win,
    End,

}
[ExecuteInEditMode]
public class StateMachine : MonoBehaviour {

    [SerializeField] List<bool> StateTable = new List<bool>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
