using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    [Header("Time visual effects")] [SerializeField]
    private float _TimeDelay = 1;

    [SerializeField] private float _TimeDrop = 1;
    [SerializeField] private float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;

    public static float TimeDelay { get; private set; }
    public static float TimeDrop { get; private set; }
    public static float TimeDropAfterDestroy { get; private set; }
    public static float TimeMove { get; private set; }
    public static float TimeRotate { get; private set; }

    private void Awake()
    {
        TimeDelay = _TimeDelay;
        TimeDrop = _TimeDrop;
        TimeDropAfterDestroy = _TimeDropAfterDestroy;
        TimeMove = _TimeMove;
        TimeRotate = _TimeRotate;
    }
}