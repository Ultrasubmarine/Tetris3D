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
    [SerializeField] StateMachine machine;

    [SerializeField] Projection myProj;
    PlaneMatrix _matrix;
    [SerializeField] HeightHandler _HeightHandler;
    [SerializeField] ElementManager _ElementManager;

    bool _isWin = false;

    [SerializeField] private Generator _Generator;
    public ElementScript NewElement { get; set; }

    [Header("Size plane")]
    [SerializeField] private int _WightPlane;

    [SerializeField] private int _HeightPlane;

    public int Height { get { return _HeightPlane - 1; } } // высота отсчитывается от 0
    public int MinCoordinat { get; private set; }


    [Header("Time visual effects")]
    [SerializeField] public float _TimeDrop = 1;
    [SerializeField] private float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;
    public float TimeRotation { get { return _TimeRotate; } }

    public planeState Mystate { get; set; }

    public GameObject TextSphere;
    public List<GameObject> TestSphereList = new List<GameObject>();

    private List<ElementScript> _elementMagrer;

    // Use this for initialization
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        Mystate = planeState.startState;//Mystate = planeState.emptyState;

        _elementMagrer = new List<ElementScript>();
 
        MinCoordinat = _WightPlane / 2 * (-1); // минимальная координата, окторая может быть в текущем поле

        NewElement = null;

        Messenger.AddListener(GameEvent.PLAY_GAME, StartGame);
        Messenger.AddListener(GameEvent.REPEAT_GAME, RepleyGame);
        Messenger.AddListener(GameEvent.WIN_GAME, WinGame);

        Messenger<float>.AddListener(GameEvent.CHANGE_TIME_DROP, ChengeTimeDrop);

    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.PLAY_GAME, StartGame);
        Messenger.RemoveListener(GameEvent.REPEAT_GAME, RepleyGame);

    }

    public void ChengeTimeDrop( float time)
    {
        //_TimeDrop = time;
    }

    //private void testSphere()
    //{
    //    // test sphere
    //    foreach (var item in TestSphereList)
    //    {
    //        var tmp = item;
    //        Destroy(tmp);

    //    }

    //    TestSphereList.Clear();
    //    for (int i = 0; i < _WightPlane; i++)
    //    {
    //        for (int j = 0; j < _HeightPlane; j++)
    //        {
    //            for (int k = 0; k < _WightPlane; k++)
    //            {
    //                if (_PlaneMatrix._matrix[i, j, k] != null)
    //                {
    //                    TestSphereList.Add(Instantiate(TextSphere, this.transform.TransformPoint(new Vector3(i - 1, j, k - 1)), Quaternion.identity));
    //                }
    //            }
    //        }
    //    }

    //    string str = "";
    //    for (int i = 0; i < 5; i++) //  YYYY
    //    {
    //        for (int j = 0; j < _WightPlane; j++) // XXX
    //        {
    //            for (int k = 0; k < _WightPlane; k++)// ZZZ
    //            {
    //                if (_PlaneMatrix._matrix[j, i, k] != null)
    //                {
    //                    str += " 0";
    //                }
    //                else
    //                    str += " 1";
    //            }
    //            str += "\n";
    //        }
    //        str += "\n";
    //    }
    //    Debug.Log(str);
    //}

    // ФУНКЦИИ ДЛЯ РАБОТЫ СО СБОРОМ КОЛЛЕКЦИЙ
    public void CheckCollected() // проверяем собранные
    {
        bool flagCollection = true;
        bool flagDestroy = false;

        flagCollection = _matrix.CollectLayers();

        if (flagCollection) // если коллекция собрана
        {
            Mystate = planeState.collectionState; // мы находимся в состоянии сбора коллекции
            _ElementManager.CutElement();
            flagDestroy = true;
        }
        
        if (flagDestroy) // Удаляем пустые элементы.
        {
         //   Debug.Log("DROP AFTER DESTROY");
          //  testSphere();
            StartCoroutine(DropAfterDestroy());
        }
        else
        {
            _HeightHandler.CheckHeight(); //CheckCurrentheight();
            Mystate = planeState.emptyState;
            machine.ChangeState(GameState2.Empty);
            Debug.Log(" SET ELEMENT MERGE");
            //  testSphere();

        }
    }

    
    public IEnumerator DropAfterDestroy()
    {
        bool flagDrop = false;
        bool checkDropState = true;

        do
        {
            flagDrop = false;

          //  Debug.Log("Element manager count =  " + _elementMagrer.Count);
            foreach (var item in _ElementManager._elementMarger)
            {
                var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
                if (empty) //если коллизии нет, элемент может падать вниз
                {
                    if (item.isBind)
                        _matrix.UnbindToMatrix(item);

                    flagDrop = true;
                    item.DropElement(this.gameObject);
                    //yield return StartCoroutine(item.DropElementVizual(item.gameObject.transform.position.y - 1.0f, TimeDropAfterDestroy)); // возвращает падение элемента
                    StartCoroutine(item.DropElementVisual(item.gameObject.transform.position.y - 1.0f, _TimeDropAfterDestroy)); // запускает падение элемента
                }
                else
                {
                    if (!item.isBind)
                        _matrix.BindToMatrix(item);
                }
            }

            // проверяем работают ли еще корутины падения элементов (проверка состояния элементов)
            while (checkDropState)
            {
                checkDropState = false;
                foreach (var item in _ElementManager._elementMarger)
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
        machine.ChangeState(GameState2.Collection);//  CheckCollected();

        _ElementManager.DestroyEmptyElement();

        yield return null;
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

                if (_matrix._matrix[x + 1, item.y, z + 1] != null)
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

                if (_matrix._matrix[x + 1, item.y, z + 1] != null)
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
    //    DestroyProection(_proectionsList);
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

                if (_matrix._matrix[item.x + 1 + 1, item.y, item.z + 1] != null)
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

                if (_matrix._matrix[item.x + 1 - 1, item.y, item.z + 1] != null)
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

                if (_matrix._matrix[item.x + 1, item.y, item.z + 1 + 1] != null)
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

                if (_matrix._matrix[item.x + 1, item.y, item.z + 1 - 1] != null)
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
        _ElementManager.DestroyAllElements();
        _HeightHandler.CheckHeight();
        StartGame();
      //  Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, 0);
    }

    private void WinGame()
    {
        _isWin = true;

        if(NewElement != null)
        {
            GameObject tmp = NewElement.gameObject;
            NewElement = null;
            Destroy(tmp);
        }

        this.enabled = false;
             
    }

}