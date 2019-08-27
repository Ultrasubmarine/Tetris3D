using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebindAnimatior : MonoBehaviour
{
	Animator _animator;
	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator>();
	}

	void OnDisable() {
		_animator.Rebind();
	}
}
