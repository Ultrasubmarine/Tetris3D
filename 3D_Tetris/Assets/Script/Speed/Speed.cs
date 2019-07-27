using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour {

    [Header("Time visual effects")]
    [SerializeField] private float _TimeDelay = 1;
    [SerializeField] private float _TimeDrop = 1;
    [SerializeField] private float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;

    static public float TimeDelay { get; private set; }
    static public float TimeDrop { get; private set; }
    static public float TimeDropAfterDestroy { get; private set; }
    static public float TimeMove { get; private set; }
    static public float TimeRotate { get; private set; }

    private void Awake() {
        TimeDelay = _TimeDelay;
        TimeDrop = _TimeDrop;
        TimeDropAfterDestroy = _TimeDropAfterDestroy;
        TimeMove = _TimeMove;
        TimeRotate = _TimeRotate;
    }
}
