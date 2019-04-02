using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Plane : MonoBehaviour {

    [Header("Size plane")]
    [SerializeField] private int _Wight;
    [SerializeField] private int _Height;
    [SerializeField] private int _LimitHeight = 11;

    public int Wight { get { return _Wight; } }
    public int Height { get { return _Height - 1; } } // высота отсчитывается от 0
    public int LimitHeight { get { return _LimitHeight; } }

    private int _currMaxHeight = 0;
    public int CurrentHeight { get { return _currMaxHeight; } }

    private int _minCoordinat;
    public int MinCoordinat { get; private set; }


   
}
