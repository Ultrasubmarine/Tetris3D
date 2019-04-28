using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum turn
{
    left,
    right
}

public enum move
{
    x = 0,
    z = 1,
    _x = 2,
    _z = 3
}

public enum planeState
{
    emptyState,
    turnState,
    moveState,
    collectionState,

    startState,
    endState
}

public class PlaneScript : Singleton<PlaneScript>
{
    [SerializeField] Projection myProj;
    [SerializeField] PlaneMatrix _PlaneMatrix;
    bool _isWin = false;

    [SerializeField] private Generator _Generator;
    public ElementScript NewElement { get; set; }

    [Header("Size plane")]
    [SerializeField] private int _WightPlane;

    [SerializeField] private int _HeightPlane;
    [SerializeField] private int _LimitHeight = 11;
    public int LimitHeight { get { return _LimitHeight; } }
    public int CurrentHeight { get { return _currMaxHeight; } }
    private int _currMaxHeight = 0;

    public int Height { get { return _HeightPlane - 1; } } // высота отсчитывается от 0
    public int Wight { get { return _WightPlane; } } // высота отсчитывается от 0
    public int MinCoordinat { get; private set; }

    [Header("Time visual effects")]
    [SerializeField] private float _TimeDrop = 1;

    [SerializeField] private float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;
    public float TimeRotation { get { return _TimeRotate; } }

    public planeState Mystate { get; private set; }

    // матриц для анализа поля.
   // public BlockScript[,,] _PlaneMatrix._matrix;
    public GameObject TextSphere;
    public List<GameObject> TestSphereList = new List<GameObject>();

    private List<ElementScript> _elementMagrer;

    [Header("Proections")]
    public GameObject ProectionObject;

    public float HeightProection = 0.1f;
    private List<GameObject> _proectionsList;

    public GameObject ProectionPotolocObject;
    public float HeightProectionPotoloc = 6;
    private List<GameObject> _potolocList;

    // Use this for initialization
    private void Start()
    {
    
        Mystate = planeState.startState;//Mystate = planeState.emptyState;

        _elementMagrer = new List<ElementScript>();
        _proectionsList = new List<GameObject>();
        _potolocList = new List<GameObject>();

        //_block = new BlockScript[_WightPlane, _HeightPlane, _WightPlane];

        MinCoordinat = _WightPlane / 2 * (-1); // минимальная координата, окторая может быть в текущем поле

        ////инициализируем пустую матрицу
        //for (int i = 0; i < _WightPlane; i++)
        //{
        //    for (int j = 0; j < _HeightPlane; j++)
        //    {
        //        for (int k = 0; k < _WightPlane; k++)
        //        {
        //            _block[i, j, k] = null;
        //        }
        //    }
        //}

        NewElement = null;

        Messenger.AddListener(GameEvent.PLAY_GAME, StartGame);
        Messenger.AddListener(GameEvent.REPEAT_GAME, RepleyGame);
        Messenger.AddListener(GameEvent.WIN_GAME, WinGame);

        Messenger<float>.AddListener(GameEvent.CHANGE_TIME_DROP, ChengeTimeDrop);

        //Debug.Log("matrix.GetUpperBound(0) =" + _block.GetUpperBound(0));
        //Debug.Log("matrix.GetUpperBound(1) =" + _block.GetUpperBound(1));
        //Debug.Log("matrix.GetUpperBound(2) =" + _block.GetUpperBound(2));
        //Debug.Log(" blo =" + _block);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.PLAY_GAME, StartGame);
        Messenger.RemoveListener(GameEvent.REPEAT_GAME, RepleyGame);
    }

    public void ChengeTimeDrop( float time)
    {
        _TimeDrop = time;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Mystate != planeState.endState && Mystate != planeState.startState && !_isWin)
        {
            if (NewElement == null && Mystate != planeState.collectionState) // проверка есть ли у нас падающий элемент
            {
                // генерируем новый элемент при помощи генератора
                GameObject generationElement = _Generator.GenerationNewElement(this);
                NewElement = generationElement.GetComponent<ElementScript>();
                NewElement.gameObject.transform.parent = this.gameObject.transform;
                StartCoroutine(ElementDrop()); // начинаем процесс падения сгенерированного элемента
            }
        }
    }

    private IEnumerator ElementDrop() // ф-я падения элемента
    {
       myProj.CreateProjection(NewElement);// VisualProection();

        while (true)
        {
            if(_isWin) //удаляем
            {
                if (NewElement != null)
                {
                    GameObject tmp = NewElement.gameObject;
                    NewElement = null;
                    Destroy(tmp);
                }
                yield break;
            }
            while (Mystate == planeState.turnState || Mystate == planeState.moveState)
            {
                yield return null;// мы не можем спустить элемент на метр ниже, пока у нас идет визуальный поворот или перемещение. ждем пока он закончится
            }

            bool collision = CheckCollisionElement(NewElement); // проверяем может ли элемент упасть на ярус ниже

            if ( _PlaneMatrix.CheckEmptyPlane(NewElement, new Vector3Int(0, -1, 0)) )//!collision)
            {
                NewElement.DropElement(this.gameObject); // логическое изменение координат падающего элемента
            }
            else
                break;

            yield return StartCoroutine(NewElement.DropElementVisual(NewElement.gameObject.transform.position.y - 1.0f, _TimeDrop));// элемент визуально падает
        }

        Destroy(_Generator.examleElement);
        while (Mystate == planeState.moveState)
        {
            yield return null;
        }
        DestroyProection(_proectionsList);
        MergerElement(); // слияние элемента и поля

        NewElement = null;
        // Mystate = planeState.emptyState;

        if (CheckLimitHeight())
        {
            Mystate = planeState.endState;
            Debug.Log("END GAME");

            Messenger.Broadcast(GameEvent.END_GAME);
            DestroyProection(_potolocList);
            yield break;
        }

        CheckCollected(); // проверяем собранные
        myProj.CreateCeiling();

        // TO DO - проверка что надо уничтожить
        yield break;
    }

    private bool CheckCollisionElement(ElementScript WhoDrop) // возвращает 1, если коллизия обнаружена.
    {
        if(WhoDrop.MyBlocks.Count ==0)
        {
            return true;
        }

        foreach (BlockScript item in /*WhoDrop.gameObject.GetComponentsInChildren<BlockScript>())//*/WhoDrop.MyBlocks)
        {
            if (!item.destroy)
            {
                if (item.y == 0)
                {
                    return true;
                }


                if (_PlaneMatrix._matrix[item.x + 1, item.y - 1, item.z + 1] != null)
                {
                    // проверяем. вдруг он упирается сам в себя
                    if (!WhoDrop.gameObject.GetComponentsInChildren<BlockScript>().Contains(_PlaneMatrix._matrix[item.x + 1, item.y - 1, item.z + 1]))//WhoDrop.MyBlocks.Contains(_block[item.x + 1, item.y - 1, item.z + 1]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // ФУНКЦИИ ДЛЯ РАБОТЫ СО СЛИЯНИЕМ ЭЛЕМЕНТА И ПОЛЯ
    private void MergerElement()
    {
        BindMatrixBlock(NewElement);

        NewElement.transform.parent = this.gameObject.transform;
        _elementMagrer.Add(NewElement);

    }

    private void testSphere()
    {
        // test sphere
        foreach (var item in TestSphereList)
        {
            var tmp = item;
            Destroy(tmp);

        }

        TestSphereList.Clear();
        for (int i = 0; i < _WightPlane; i++)
        {
            for (int j = 0; j < _HeightPlane; j++)
            {
                for (int k = 0; k < _WightPlane; k++)
                {
                    if (_PlaneMatrix._matrix[i, j, k] != null)
                    {
                        TestSphereList.Add(Instantiate(TextSphere, this.transform.TransformPoint(new Vector3(i - 1, j, k - 1)), Quaternion.identity));
                    }
                }
            }
        }

        string str = "";
        for (int i = 0; i < 5; i++) //  YYYY
        {
            for (int j = 0; j < _WightPlane; j++) // XXX
            {
                for (int k = 0; k < _WightPlane; k++)// ZZZ
                {
                    if (_PlaneMatrix._matrix[j, i, k] != null)
                    {
                        str += " 0";
                    }
                    else
                        str += " 1";
                }
                str += "\n";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    private void BindMatrixBlock(ElementScript element)
    {
        int x, y, z;

        foreach (BlockScript item in element.MyBlocks)
        {
            if (item == null || item.destroy)
                continue;
            x = item.x;
            y = item.y;
            z = item.z;

            _PlaneMatrix._matrix[x + 1, y, z + 1] = item;
        }

        element.isBind = true;
    }

    private void UnbindMatrixBlock(ElementScript element) // отвязывает блоки данного элемента от матрицы поля
    {
        int x, y, z;
        foreach (BlockScript item in element.MyBlocks)
        {
            if (item == null || item.destroy)
                continue;

            x = item.x;
            y = item.y;
            z = item.z;

            _PlaneMatrix._matrix[x + 1, y, z + 1] = null;
        }

        element.isBind = false;
    }

    // ФУНКЦИИ ДЛЯ РАБОТЫ СО СБОРОМ КОЛЛЕКЦИЙ
    private void CheckCollected() // проверяем собранные
    {
        bool flagCollection = true;
        bool flagDestroy = false;

        for (int y = 0; y < _HeightPlane; y++) // проверяем все в поскоскости XZ
        {
            flagCollection = true;

            for (int x = 0; x < _WightPlane && flagCollection; x++)
            {
                for (int z = 0; z < _WightPlane; z++)
                {
                    if (_PlaneMatrix._matrix[x, y, z] == null) // если в этом слое есть пустое место, значит колелкция не собрана
                    {
                        flagCollection = false;
                        break;
                    }
                }
            }

            if (flagCollection) // если коллекция собрана
            {
                Mystate = planeState.collectionState; // мы находимся в состоянии сбора коллекции
                DestroyLayer(y);

                int k = 0;
                int countK = _elementMagrer.Count();
                while(k < countK)
                {
                    ElementScript b = _elementMagrer[k].CheckUnion();
                    if( b != null)
                    {
                        UnbindMatrixBlock(b);
                        UnbindMatrixBlock(_elementMagrer[k]);

                        Debug.Log("Create element +++");
                        _elementMagrer.Add(b);
                        b.transform.parent = gameObject.transform;
                        countK++;
                    }
                    k++;
                }
                // TO DO DestroyVizyal  корутина для отображения уничтожения
                flagDestroy = true;
            }
        }

        if (flagDestroy) // Удаляем пустые элементы.
        {
         //   Debug.Log("DROP AFTER DESTROY");
          //  testSphere();
            StartCoroutine(DropAfterDestroy());
        }
        else
        {
            CheckCurrentheight();
            Mystate = planeState.emptyState;

          //  testSphere();

        }
    }

    private void DestroyLayer(int y)
    {
       // Debug.Log("Destroy layer y=" + y);
        Messenger<int>.Broadcast(GameEvent.DESTROY_LAYER, y);
        for (int x = 0; x < _WightPlane; x++)
        {
            for (int z = 0; z < _WightPlane; z++)
            {
                GameObject tmp = _PlaneMatrix._matrix[x, y, z].gameObject;
                var ggg = _PlaneMatrix._matrix[x, y, z];
                _PlaneMatrix._matrix[x, y, z].destroy = true;
               //
               //  (_block[x, y, z]);
                _PlaneMatrix._matrix[x, y, z] = null;

                tmp.GetComponentInParent<ElementScript>().DeleteBlock(ggg);
              //  tmp.SetActive(false);
                //Destroy(tmp);
            }
        }

    }

    private IEnumerator DropAfterDestroy()
    {
        bool flagDrop = false;
        bool checkDropState = true;

        do
        {
            flagDrop = false;

          //  Debug.Log("Element manager count =  " + _elementMagrer.Count);
            foreach (var item in _elementMagrer)
            {
                if (!CheckCollisionElement(item)) //если коллизии нет, элемент может падать вниз
                {
                    if (item.isBind)
                        UnbindMatrixBlock(item);

                    flagDrop = true;
                    item.DropElement(this.gameObject);
                    //yield return StartCoroutine(item.DropElementVizual(item.gameObject.transform.position.y - 1.0f, TimeDropAfterDestroy)); // возвращает падение элемента
                    StartCoroutine(item.DropElementVisual(item.gameObject.transform.position.y - 1.0f, _TimeDropAfterDestroy)); // запускает падение элемента
                }
                else
                {
                    if (!item.isBind)
                        BindMatrixBlock(item);
                }
            }

            // проверяем работают ли еще корутины падения элементов (проверка состояния элементов)
            while (checkDropState)
            {
                checkDropState = false;
                foreach (var item in _elementMagrer)
                {
                    if (item.isDrop)
                    {
                        checkDropState = true;
                        yield return null;
                        break;
                    }
                }
              //  Debug.Log("*");
            }
            yield return new WaitForSeconds(_TimeDropAfterDestroy);
        }
        while (flagDrop); // проверяем что бы все упало, пока оно может падать

        myProj.CreateCeiling() ;
        CheckCollected();

       DestroyEmptyElement();

        yield return null;
    }

    private void DestroyEmptyElement()
    {
        // проверка пустых элементов
        for (int i = 0; i < _elementMagrer.Count;)
        {
            if (_elementMagrer[i].CheckEmptyElement())//_elementMagrer[i].gameObject.transform.childCount == 0)
            {
                GameObject tmp = _elementMagrer[i].gameObject;
                _elementMagrer.Remove(_elementMagrer[i]);
                Destroy(tmp);
            }
            else
            {
                i++;
            }
        }
    }

    // ФУНКЦИИ ДЛЯ ПРОВЕРКИ ВЫСОТЫ
    private bool CheckLimitHeight()
    {
        for (int i = 0; i < _WightPlane; i++)
        {
            for (int j = 0; j < _WightPlane; j++)
            {
                if (_PlaneMatrix._matrix[i, _LimitHeight, j] != null)
                    return true;
            }
        }
        return false;
    }

    private void CheckCurrentheight()
    {
        //Debug.Log("CHECK Height");
        _currMaxHeight = 0;
        if (Mystate == planeState.endState)
        {
            Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, 0);
            return;
        }
        for (int y = 0; y < _LimitHeight; y++)
        {
            for (int x = 0; x < _WightPlane; x++)
            {
                for (int z = 0; z < _WightPlane; z++)
                {
                    if (_PlaneMatrix._matrix[x, y, z] != null)
                    {
                        _currMaxHeight = y;
                        //Debug.Log("Max = " + _currMaxHeight);                      
                        continue;
                    }
                }
            }
        }
        Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, _currMaxHeight + 1);
    }

    // ФУНКЦИИ ПОВОРОТА ЭЛЕМЕНТА
    public bool TurnElement(turn napravl) // возвращает разрешение на поворот камеры, если мы можем повернуть элемент
    {
        if (Mystate != planeState.emptyState)
            return false; // ХММММММММММММММММ,,, ЭТО НАВЕРНОЕ НЕ ОЧЕНЬ

        if (ChekTurnElement(napravl))
        {
            NewElement.TurnElement(napravl, this.gameObject);

            StartCoroutine(TurnElementVizual(napravl));
            return true;
        }

        return false;
    }

    private bool ChekTurnElement(turn direction) // проверяет возможность поворота падающего элемента
    {
        int x, z;
        if (direction == turn.left)
        {
            foreach (var item in NewElement.MyBlocks)
            {
                // по правилу поворота
                x = item.z;
                z = -item.x;

                if (_PlaneMatrix._matrix[x + 1, item.y, z + 1] != null)
                    return false;
            }
        }
        else
        {
            foreach (var item in NewElement.MyBlocks)
            {
                // по правилу поворота
                x = -item.z;
                z = item.x;

                if (_PlaneMatrix._matrix[x + 1, item.y, z + 1] != null)
                    return false;
            }
        }

        return true;
    }

    private IEnumerator TurnElementVizual(turn direction) // начинаем визуальный поворот
    {
        int rotate;
        if (direction == turn.left)
            rotate = 90;
        else
            rotate = -90;

        Mystate = planeState.turnState;
        DestroyProection(_proectionsList);
        yield return StartCoroutine(NewElement.TurnElementVizual(rotate, _TimeRotate, this.gameObject));

        myProj.CreateProjection(NewElement);// VisualProection();   VisualProection();
        Mystate = planeState.emptyState;
        yield return null;
    }

    // ФУНКЦИИ ПЕРЕМЕЩЕНИЯ ЭЛЕМЕНТА
    public void MoveElement(move direction)
    {
        if (Mystate != planeState.emptyState)
            return; // ХММММММММММММММММ,,, ЭТО НАВЕРНОЕ НЕ ОЧЕНЬ

        if (CheckMoveElement(direction))
        {
            NewElement.MoveElement(direction);
            StartCoroutine(MoveElementVizual(direction));

            myProj.CreateProjection(NewElement);// VisualProection();   VisualProection();
            //   Debug.Log("ПЕРЕМЕСТИЛИ");
        }
    }

    private bool CheckMoveElement(move direction) // 1- можно переместить 2- нельзя
    {
        if (NewElement == null)
            return false;

        if (direction == move.x)
        {
            foreach (var item in NewElement.MyBlocks)
            {
                if (item.x == Mathf.Abs(MinCoordinat))
                    return false;

                if (_PlaneMatrix._matrix[item.x + 1 + 1, item.y, item.z + 1] != null)
                    return false;
            }

            return true;
        }
        else if (direction == move._x)
        {
            foreach (var item in NewElement.MyBlocks)
            {
                if (item.x == MinCoordinat)
                    return false;

                if (_PlaneMatrix._matrix[item.x + 1 - 1, item.y, item.z + 1] != null)
                    return false;
            }

            return true;
        }
        else if (direction == move.z)
        {
            foreach (var item in NewElement.MyBlocks)
            {
                if (item.z == Mathf.Abs(MinCoordinat))
                    return false;

                if (_PlaneMatrix._matrix[item.x + 1, item.y, item.z + 1 + 1] != null)
                    return false;
            }

            return true;
        }
        else if (direction == move._z)
        {
            foreach (var item in NewElement.MyBlocks)
            {
                if (item.z == MinCoordinat)
                    return false;

                if (_PlaneMatrix._matrix[item.x + 1, item.y, item.z + 1 - 1] != null)
                    return false;
            }

            return true;
        }

        return true;
    }

    private IEnumerator MoveElementVizual(move direction)
    {
        Vector3 vectorDirection;

        if (direction == move.x)
            vectorDirection = new Vector3(1.0f, 0f, 0f);
        else if (direction == move._x)
            vectorDirection = new Vector3(-1.0f, 0f, 0f);
        else if (direction == move.z)
            vectorDirection = new Vector3(0f, 0f, 1.0f);
        else // (direction == move._z)
            vectorDirection = new Vector3(0f, 0f, -1.0f);

        Mystate = planeState.moveState;
        yield return StartCoroutine(NewElement.MoveElementVisual(vectorDirection, _TimeMove));

        Mystate = planeState.emptyState;
        yield return null;
    }

    // ФУНКЦИИ ДЛЯ РАБОТЫ С ПРОЕКЦИЯМИ
    private void VisualProection()
    {
        bool flagCreate = false;

        DestroyProection(_proectionsList);

        foreach (var item in NewElement.MyBlocks)
        {
            flagCreate = false;

            foreach (var proectionItem in _proectionsList) // проверяем есть ли уже проекция на эту клетку поля
            {
                if (proectionItem.gameObject.transform.position.x == item.x && proectionItem.gameObject.transform.position.z == item.z)
                    flagCreate = true; // мы уже такую клетку прописали
            }

            if (!flagCreate)
            {
                GameObject tmp = Instantiate(ProectionObject, this.gameObject.transform);

                float y = -1;
                for (int i = _HeightPlane - 1; i > -1; i--)
                {
                    if (_PlaneMatrix._matrix[(int)item.x + 1, i, (int)item.z + 1] != null)
                    {
                        y = i;
                        break;
                    }
                }

                tmp.transform.position = new Vector3(item.x, (y + 1.0f + HeightProection), item.z);
                _proectionsList.Add(tmp);
            }
        }
    }

    private void DestroyProection(List<GameObject> proectionList)
    {
        GameObject tmpDestroy;
        foreach (var item in proectionList)//_proectionsList)
        {
            tmpDestroy = item;
            Destroy(tmpDestroy);
        }

        proectionList.Clear(); //_proectionsList.Clear();
    }

    private void CheckPotoloc()
    {
        DestroyProection(_potolocList);

        for (int x = 0; x < _WightPlane; x++) // подумать насчет 3-ки
        {
            for (int z = 0; z < _WightPlane; z++)
            {
                if (_PlaneMatrix._matrix[x, (int)(_LimitHeight - 1), z] != null || _PlaneMatrix._matrix[x, (int)(_LimitHeight - 2), z] != null)
                {
                    GameObject tmp = Instantiate(ProectionPotolocObject, this.gameObject.transform);

                    tmp.transform.position = new Vector3(x - 1, (_LimitHeight + HeightProection), z - 1);
                    _potolocList.Add(tmp);
                }
            }
        }
    }

    // ФУНКЦИИ ВОСПРОИЗВЕДЕНИЯ САМОЙ ИГРЫ
    public void StartGame()
    {
        Debug.Log("PLAY");
        if (Mystate == planeState.startState || Mystate == planeState.endState)
        {
            Mystate = planeState.emptyState;
        }
    }

    private void RepleyGame()
    {
        while (_elementMagrer.Count > 0)
        {
            ElementScript tmp = _elementMagrer[0];
            UnbindMatrixBlock(tmp);
            _elementMagrer.Remove(tmp);
            Destroy(tmp.gameObject);
        }
        CheckCurrentheight();
        StartGame();
        Debug.Log("Current max H " + _currMaxHeight);
      //  Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, 0);
    }

    private void WinGame()
    {
        _isWin = true;
        Debug.Log("END END? PLANE KNOW");

        if(NewElement != null)
        {
            GameObject tmp = NewElement.gameObject;
            NewElement = null;
            Destroy(tmp);
        }

        this.enabled = false;
            
        
    }

    public int MinHeightInCoordinates(int x, int z)
    {
        for (int y = _PlaneMatrix._matrix.GetUpperBound(1) - 1; y >= 0; --y)
        {
            if (_PlaneMatrix._matrix[x, y, z] != null)
                return y + 1;
        }
        return 0;
    }

}