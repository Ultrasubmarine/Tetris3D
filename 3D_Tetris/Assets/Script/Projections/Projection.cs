using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using IntegerExtension;
using UnityEngine.Networking.NetworkSystem;

public class Projection : MonoBehaviour {

    PlaneMatrix _matrix;

    public const int PROECTIONS = 1;
    public const int CEILING = 2;

    [Header(" Проекция ")]
    [SerializeField] GameObjectPool _PoolProjection;
    [SerializeField] float _HeightProection = 0.1f;
    private List<GameObject> _proectionsList = new List<GameObject>();

    [Header(" Потолок ")] // ceiling - потолок
    [SerializeField] GameObjectPool _PoolСeiling;
    [SerializeField] int _MinimumLayerHeight;
    private List<GameObject> _ceilingList = new List<GameObject>();

    private void Awake() {
        Messenger<Element>.AddListener(GameEvent.CREATE_NEW_ELEMENT.ToString(), CreateProjection);
        Messenger<Element>.AddListener(GameEvent.TURN_ELEMENT.ToString(), CreateProjection);
        Messenger<Element>.AddListener(GameEvent.MOVE_ELEMENT.ToString(), CreateProjection);
        
        Messenger<int,int>.AddListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);

        Messenger<Element>.AddListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
    }

    private void OnDestroy() {
        Messenger<Element>.RemoveListener(GameEvent.CREATE_NEW_ELEMENT.ToString(), CreateProjection);
        Messenger<Element>.RemoveListener(GameEvent.TURN_ELEMENT.ToString(), CreateProjection);
        Messenger<Element>.RemoveListener(GameEvent.MOVE_ELEMENT.ToString(), CreateProjection);
        
        Messenger<int,int>.RemoveListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);
       
        Messenger<Element>.RemoveListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
    }

    private void Start() {
        _matrix = PlaneMatrix.Instance;
    }

    public void CreateProjection(Element obj) {

        Destroy(PROECTIONS);

        var positionProjection = obj.MyBlocks.Select(b => b.XZ).Distinct();
        foreach (var item in positionProjection)
        {
            float y = _matrix.MinHeightInCoordinates(item.x.ToIndex(), item.z.ToIndex());

            var posProjection = new Vector3(item.x, (y + _HeightProection), item.z);

           _proectionsList.Add(_PoolProjection.CreateObject(posProjection));
        }
    }

    public void CreateCeiling(int limit, int current) {

        Destroy(CEILING);

        if ( current < _MinimumLayerHeight)
            return;

        for(int x=0; x< _matrix.Wight; x++) {
            for (int z = 0; z < _matrix.Wight; z++) {
                
                int y = _matrix.MinHeightInCoordinates(x, z);
                if(y >= _MinimumLayerHeight)
                   _ceilingList.Add(_PoolСeiling.CreateObject( new Vector3(x.ToCoordinat(),_matrix.LimitHeight + _HeightProection,z.ToCoordinat()) ));
            }
        }
    }

    public void DeleteProjection(Element element)
    {
        Destroy(PROECTIONS);
    }
    private void Destroy(int typeObject /* const PROECTIONS or CEILING*/ ) {
        List<GameObject> list;
        GameObjectPool pool;

        switch (typeObject) {
            case PROECTIONS: {
                    list = _proectionsList;
                    pool = _PoolProjection;
                    break;
                }
            case CEILING: {
                    list = _ceilingList;
                    pool = _PoolСeiling;
                    break;
                }
            default: {
                    Debug.Log("ERROR: value proections not found (Projection.cs)");
                    return;
                    break;
                }
        }
        DestroyList(list, pool);
    }

    private void DestroyList(List<GameObject> list, GameObjectPool pool) {
        foreach (var item in list) {
            pool.DestroyObject(item);
        }
        list.Clear();
    }
}
