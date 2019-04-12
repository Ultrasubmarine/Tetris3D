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

    private void BindBlock(ElementScript element) {

        ChengeBind(element, true);
    }

    private void UnbindBlock(ElementScript element) // отвязывает блоки данного элемента от матрицы поля
    {
        ChengeBind(element, false);
    }

    private void ChengeBind(ElementScript element, bool bind) {

        int x, y, z;
        foreach (BlockScript item in element.MyBlocks) {
            if (item == null || item.destroy)
                continue;

            x = item.x;
            y = item.y;
            z = item.z;

            _matrix[x + 1, y, z + 1] = bind ? item: null ;
        }

        element.isBind = bind;
    }

    private void CheckCollected() // проверяем собранные
    {
        bool flagCollection = true;
        bool flagDestroy = false;

        for (int y = 0; y < _Height; y++) // проверяем все в поскоскости XZ
        {
            flagCollection = CheckLayerCollect(y);

            if (flagCollection) // если коллекция собрана
            {
                //Mystate = planeState.collectionState; // мы находимся в состоянии сбора коллекции
                //DestroyLayer(y);

                //int k = 0;
                //int countK = _elementMagrer.Count();
                //while (k < countK) {
                //    ElementScript b = _elementMagrer[k].CheckUnion();
                //    if (b != null) {
                //        UnbindBlock(b);
                //        UnbindBlock(_elementMagrer[k]);

                //        Debug.Log("Create element +++");
                //        _elementMagrer.Add(b);
                //        b.transform.parent = gameObject.transform;
                //        countK++;
                //    }
                //    k++;
                //}
                // TO DO DestroyVizyal  корутина для отображения уничтожения
                flagDestroy = true;
            }
        }
    }

    private bool CheckLayerCollect( int layer) {

        bool flagCollection = true;
        for (int x = 0; x < _Wight && flagCollection; x++) {
            for (int z = 0; z < _Wight; z++) {
                if (_matrix[x, layer, z] == null) // если в этом слое есть пустое место, значит колелкция не собрана
                {
                    flagCollection = false;
                    return flagCollection;
                    break;
                }
            }
        }
        return flagCollection;
    }
        public void DestroyLayer( int layer)
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
