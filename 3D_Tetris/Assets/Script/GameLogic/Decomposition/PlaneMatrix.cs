using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntegerExtension;

public class PlaneMatrix : Singleton<PlaneMatrix> {

    public BlockScript[,,] _matrix;

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
        ExtensionMetodsForMatrix.SetSizePlane(_Wight);
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

    public bool CheckEmptyPlane( ElementScript element, Vector3Int direction) {

        if (element.MyBlocks.Count == 0) 
            return false;
        
        Vector3Int newCoordinat;
        foreach (BlockScript item in element.MyBlocks) {
            if (!item.destroy) {

                newCoordinat = new Vector3Int(item.x, item.y, item.z) + direction;

                if (newCoordinat.OutOfCoordinatLimit()) 
                    return false;               

                if (_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()] != null) {
                    if (!element.isBind)
                        return false;
                    if (!element.MyBlocks.Contains(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()])) 
                        return false;                
                }
            }
        }
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
