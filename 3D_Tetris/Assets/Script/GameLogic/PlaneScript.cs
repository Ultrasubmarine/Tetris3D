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

    PlaneMatrix _matrix;
    [SerializeField] HeightHandler _HeightHandler;
    [SerializeField] ElementManager _ElementManager;

    bool _isWin = false;

    [SerializeField] private Generator _Generator;
    public Element NewElement { get; set; }

    [Header("Size plane")]
    [SerializeField] private int _WightPlane;

    [SerializeField] private int _HeightPlane;

    public int Height { get { return _HeightPlane - 1; } } // высота отсчитывается от 0
    public int MinCoordinat { get; private set; }


    [Header("Time visual effects")]
    [SerializeField] public float _TimeDelay = 1;
    [SerializeField] public float _TimeDrop = 1;
    [SerializeField] public float _TimeDropAfterDestroy = 1;
    [SerializeField] private float _TimeMove = 1;
    [SerializeField] private float _TimeRotate = 1;
    public float TimeRotation { get { return _TimeRotate; } }

    public planeState Mystate { get; set; }

    public GameObject TextSphere;
    public List<GameObject> TestSphereList = new List<GameObject>();

    // Use this for initialization
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        Mystate = planeState.startState;//Mystate = planeState.emptyState;

 

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