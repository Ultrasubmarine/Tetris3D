using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMatrix : Singleton<PlaneMatrix> {

    BlockScript[,,] _matrix;

    [Header("Size plane")]
    [SerializeField] int _LimitHeight = 11;
    [SerializeField] int _Wight;
    [SerializeField] int _Height;


    public int Wight { get { return _Wight;  } }
    public int Height { get { return _Height - 1; } } // высота отсчитывается от 0

    public int LimitHeight { get { return _LimitHeight; } }
    public int CurrentHeight { get; private set; }

    public static int MinCoordinat { get; private set; }

    private void Awake()
    {
        _matrix = new BlockScript[_Wight, _Height, _Wight];

        MinCoordinat = _Wight / 2 * (-1); // минимальная координата, окторая может быть в текущем поле

        //инициализируем пустую матрицу
        for (int i = 0; i < _Wight; i++)
        {
            for (int j = 0; j < _Height; j++)
            {
                for (int k = 0; k < _Wight; k++)
                {
                    _matrix[i, j, k] = null;
                }
            }
        }
    }
    public PlaneMatrix()
    {
        MinCoordinat = Wight / 2 * (-1); // минимальная координата, окторая может быть в текущем поле 
    }

    public bool CheckCollisionElement()
    {
        return true;
    }

    public void BindMatrixBlock()
    {

    }
    public void UnbindMatrixBlock()
    {

    } 

    public void CheckCollected()
    {

    }

    public void DestroyLayer()
    {

    }

    public bool CheckEmptyPlaceInMatrix( IEnumerable places)
    {
        foreach (var place in places)
        {
            if (place != null)
                return false;
        }
        return true;
    }


    public int MinHeightInCoordinates(int x, int z)
    {
        for (int y = _matrix.GetUpperBound(1) - 1; y >= 0; --y)
        {
            if (_matrix[x - MinCoordinat, y, z - MinCoordinat] != null)
                return y+1;
        }
        return 0;        
    }
}
