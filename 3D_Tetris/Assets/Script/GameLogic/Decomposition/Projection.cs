using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using IntegerExtension;

public class Projection : MonoBehaviour {

    PlaneMatrix _matrix;
    [SerializeField] HeightHandler _Heighthandler;//PlaneScript _plane;
    // TO DO - статическая переменная - высота сцены.
    //  int _HeightPlane;

    public const int PROECTIONS = 1;
    public const int CEILING = 2;

    [Header(" Проекция ")]
    [SerializeField] ObjectPool _PoolProjection;
    [SerializeField] float _HeightProection = 0.1f;
    private List<GameObject> _proectionsList = new List<GameObject>();

    [Header(" Потолок ")] // ceiling - потолок
    [SerializeField] ObjectPool _PoolСeiling;
    [SerializeField] int MinimumLayerHeight;
    private List<GameObject> _ceilingList = new List<GameObject>();

    private void Awake() {
        Messenger<Element>.AddListener(GameEvent.CREATE_NEW_ELEMENT, CreateProjection);
        Messenger<Element>.AddListener(GameEvent.TURN_ELEMENT, CreateProjection);
        Messenger<Element>.AddListener(GameEvent.MOVE_ELEMENT, CreateProjection);
    }

    private void OnDestroy() {
        Messenger<Element>.RemoveListener(GameEvent.CREATE_NEW_ELEMENT, CreateProjection);
        Messenger<Element>.RemoveListener(GameEvent.TURN_ELEMENT, CreateProjection);
        Messenger<Element>.RemoveListener(GameEvent.MOVE_ELEMENT, CreateProjection);
    }

    private void Start() {
        _matrix = PlaneMatrix.Instance;
    }
    // ФУНКЦИИ ДЛЯ РАБОТЫ С ПРОЕКЦИЯМИ
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

    public void CreateCeiling() {

        if( _ceilingList.Count > 0)
        Destroy(CEILING);

        if (_matrix.CurrentHeight < MinimumLayerHeight)
            return;

        for(int x=0; x< _matrix.Wight; x++) {
            for (int z = 0; z < _matrix.Wight; z++) {
                
                int y = _matrix.MinHeightInCoordinates(x, z);
                if(y >= MinimumLayerHeight)
                    _ceilingList.Add(_PoolСeiling.CreateObject( new Vector3(x.ToCoordinat(),_matrix.LimitHeight + _HeightProection,z.ToCoordinat()) ));
            }
        }
    }

    public void Destroy(int typeObject /* const PROECTIONS or CEILING*/ ) {
        List<GameObject> list;
        ObjectPool pool;

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

    private void DestroyList(List<GameObject> list, ObjectPool pool) {
        GameObject tmpDestroy;
        foreach (var item in list) {
            tmpDestroy = item;
            pool.DestroyObject(tmpDestroy);
        }

        list.Clear();
    }
}
