using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour {

    [Header("Time visual effects")]
    [SerializeField] public float _TimeDelay = 1;
    [SerializeField] public float _TimeDrop = 1;
    [SerializeField] public float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;

}
