using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntegerExtension;

public class PlaneMatrix : Singleton<PlaneMatrix> {

    public BlockScript[,,] _matrix;

    [Header("Size plane")]
    int _LimitHeight = 18; 
    [SerializeField] int _Wight;
    [SerializeField] int _Height;


    public int Wight { get { return _Wight;  } }
    public int Height { get { return _Height - 1; } } // высота отсчитывается от 0

    public int LimitHeight { get { return _LimitHeight; } }
    public int CurrentHeight { get; private set; }

    public static int MinCoordinat { get; private set; }

    protected override void Init() {
        ExtensionMetodsForMatrix.SetSizePlane(_Wight);
        _matrix = new BlockScript[_Wight, _Height, _Wight];

        MinCoordinat = _Wight / 2 * (-1); // минимальная координата, окторая может быть в текущем поле

        //инициализируем пустую матрицу
        for (int i = 0; i < _Wight; i++) {
            for (int j = 0; j < _Height; j++) {
                for (int k = 0; k < _Wight; k++) {
                    _matrix[i, j, k] = null;
                }
            }
        }
    }
 
    public void SetLimitHeight( int limit) {
        _LimitHeight = limit;
    }

    public bool CheckEmptyPlaсe( ElementScript element, Vector3Int direction) {

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

    public void BindToMatrix(ElementScript element) {

        int x, y, z;
        foreach (BlockScript item in element.MyBlocks) {
            if (item == null || item.destroy)
                continue;
            x = item.x;
            y = item.y;
            z = item.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = item;
        }
        element.isBind = true;
    }

    public void UnbindToMatrix(ElementScript element) {

        int x, y, z;
        foreach (BlockScript item in element.MyBlocks) {
            if (item == null || item.destroy)
                continue;
            x = item.x;
            y = item.y;
            z = item.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = null;
        }
        element.isBind = false;
    } 

    public bool CollectLayers() {

        bool flag = false; ;
        for (int y = 0; y < _LimitHeight; y++) {
            if (CheckCollectedInLayer(y)) {
                DestroyLayer(y);
                flag = true;
            }
        }
        return flag;
    }
    public bool CheckCollectedInLayer(int layer) {

        for (int x = 0; x < Wight; x++) {
            for (int z = 0; z < Wight; z++) {
                if (_matrix[x, layer, z] == null) // если в этом слое есть пустое место, значит колелкция не собрана
                {
                    return false;    
                }
            }
        }
        return true;       
    }

    public void DestroyLayer(int layer)
    {
        Messenger<int>.Broadcast(GameEvent.DESTROY_LAYER, layer);
        for (int x = 0; x < Wight; x++) {
            for (int z = 0; z < Wight; z++) {

                GameObject tmp = _matrix[x, layer, z].gameObject;
                var ggg = _matrix[x, layer, z];
                _matrix[x, layer, z].destroy = true;
                _matrix[x, layer, z] = null;
                tmp.GetComponentInParent<ElementScript>().DeleteBlock(ggg);             
            }
        }
    }

    public int MinHeightInCoordinates(int x, int z)
    {
        for (int y = _matrix.GetUpperBound(1) - 1; y >= 0; --y)
        {
            if (_matrix[x, y, z] != null)
                return y+1;
        }
        return 0;        
    }
}
