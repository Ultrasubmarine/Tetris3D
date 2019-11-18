using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;

public class Projection : MonoBehaviour
{
    private PlaneMatrix _matrix;

    public const int PROJECTIONS = 1;
    public const int CEILING = 2;

    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _HeightProjection = 0.1f;
    
    private List<GameObject> _projectionsList = new List<GameObject>();
    private Pool<GameObject> _pPool;
    
    [Header(" Потолок ")] [SerializeField] private GameObjectPool _PoolCeiling;
    [SerializeField] private int _MinimumLayerHeight;
    private List<GameObject> _ceilingList = new List<GameObject>();

    private TetrisFSM _fsm;
    private void Awake()
    {
//        Messenger<Element>.AddListener(GameEvent.CREATE_NEW_ELEMENT.ToString(), CreateProjection);
//        Messenger<Element>.AddListener(GameEvent.TURN_ELEMENT.ToString(), CreateProjection);
//        Messenger<Element>.AddListener(GameEvent.MOVE_ELEMENT.ToString(), CreateProjection);
//        
//        Messenger<int,int>.AddListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);
//
//        Messenger<Element>.AddListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
//        
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearAllProjections);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.Win, ClearAllProjections);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.End, ClearAllProjections);
    }
    private void OnDestroy()
    {
//        Messenger<Element>.RemoveListener(GameEvent.CREATE_NEW_ELEMENT.ToString(), CreateProjection);
//        Messenger<Element>.RemoveListener(GameEvent.TURN_ELEMENT.ToString(), CreateProjection);
//        Messenger<Element>.RemoveListener(GameEvent.MOVE_ELEMENT.ToString(), CreateProjection);
//        
//        Messenger<int,int>.RemoveListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);
//       
//        Messenger<Element>.RemoveListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
//        
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearAllProjections);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Win, ClearAllProjections);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.End, ClearAllProjections);
    }

    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _fsm = RealizationBox.Instance.FSM;

        _pPool = new Pool<GameObject>(_prefab);
        Invoke(nameof(LastStart), 1f);
    }

    private void LastStart()
    {
        ElementData.NewElementUpdate += CreateProjection;
        
        _fsm.AddListener(TetrisState.Move, CreateProjection);
        _fsm.AddListener(TetrisState.MergeElement, () => Destroy(PROJECTIONS));
    }
    
    
    private void CreateProjection()
    {
        Element obj = ElementData.NewElement;

        foreach (var item in _projectionsList)
        {
            _pPool.Push(item);
        }
        _projectionsList.Clear();
        
        var positionProjection = obj.MyBlocks.Select(b => b.XZ).Distinct();
        
        foreach (var item in positionProjection)
        {
            float y = _matrix.MinHeightInCoordinates(item.x.ToIndex(), item.z.ToIndex());

            var posProjection = new Vector3(item.x, y + _HeightProjection, item.z);

            var o = _pPool.Pop(true);
            o.transform.position = posProjection;
            _projectionsList.Add(o);
        }
    }

    private void CreateCeiling(int limit, int current)
    {
        Destroy(CEILING);

        if (current < _MinimumLayerHeight)
            return;

        for (var x = 0; x < _matrix.wight; x++)
        for (var z = 0; z < _matrix.wight; z++)
        {
            var y = _matrix.MinHeightInCoordinates(x, z);
            if (y >= _MinimumLayerHeight)
                _ceilingList.Add(_PoolCeiling.CreateObject(new Vector3(x.ToCoordinat(),
                    _matrix.limitHeight + _HeightProjection, z.ToCoordinat())));
        }
    }

    private void DeleteProjection(Element element)
    {
        Destroy(PROJECTIONS);
    }

    private void Destroy(int typeObject /* const PROECTIONS or CEILING*/)
    {
        List<GameObject> list;
        GameObjectPool pool;

        switch (typeObject)
        {
            case PROJECTIONS:
            {
                foreach (var item in _projectionsList)
                {
                    _pPool.Push(item);
                }
                _projectionsList.Clear();
                break;
            }
            case CEILING:
            {
                list = _ceilingList;
                pool = _PoolCeiling;
                break;
            }
            default:
            {
                Debug.Log("ERROR: value proections not found (Projection.cs)");
                return;
                break;
            }
        }
    }

    private void DestroyList(List<GameObject> list, GameObjectPool pool)
    {
        foreach (var item in list) pool.DestroyObject(item);
        list.Clear();
    }

    private void ClearAllProjections()
    {
        Destroy(PROJECTIONS);
        Destroy(CEILING);
    }
}