using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public static class PlaneSize {

    [Header("Size plane")]
    public static int _LimitHeight = 11;

    public static int Wight { get; private set; }
    public static int Height { get { return Height - 1; } } // высота отсчитывается от 0
    public static int LimitHeight { get { return _LimitHeight; } }  
    public static int CurrentHeight { get; private set; }

    public static int MinCoordinat { get; private set; }

    static PlaneSize() {
        MinCoordinat = Wight / 2 * (-1); // минимальная координата, окторая может быть в текущем поле 
    }
    
}
