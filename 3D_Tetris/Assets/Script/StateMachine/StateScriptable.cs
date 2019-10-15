using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum State
{
	A,B,C,
} 

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class StateScriptable : ScriptableObject
{
	public int count;
	public State state;

	[SerializeField] UnityEvent _ev;
}
