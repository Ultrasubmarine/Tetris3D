using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitBeforeEvent : MonoBehaviour
{
	[SerializeField] float _Timer;
	[SerializeField] UnityEvent _InvokeAfterTime;

	void Start() {
		StartCoroutine(Timer());
	}

	IEnumerator Timer() {
		yield return new WaitForSeconds( _Timer);
		_InvokeAfterTime.Invoke();
	}
}
