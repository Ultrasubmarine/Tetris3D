using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Projection : Singleton<Projection> {

    // PlaneMatrix _plane;
    [SerializeField] PlaneScript _plane;
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

    // ФУНКЦИИ ДЛЯ РАБОТЫ С ПРОЕКЦИЯМИ
    public void CreateProjection(ElementScript obj) {

        Destroy(PROECTIONS);

        var positionProjection = obj.MyBlocks.Select(b => b.XZ).Distinct();
        foreach (var item in positionProjection)
        {
            float y = _plane.MinHeightInCoordinates(item.x - _plane.MinCoordinat, item.y -_plane.MinCoordinat);

            var posProjection = new Vector3(item.x, (y + _HeightProection), item.y);

            _proectionsList.Add(_PoolProjection.CreateObject(posProjection));
        }
    }

    public void CreateCeiling() {

        if( _ceilingList.Count > 0)
        Destroy(CEILING);

        if (_plane.CurrentHeight < MinimumLayerHeight)
            return;

        for(int x=0; x< _plane.Wight; x++) {
            for (int z = 0; z < _plane.Wight; z++) {
                
                int y = _plane.MinHeightInCoordinates(x, z);
                if(y >= MinimumLayerHeight)
                    _ceilingList.Add(_PoolСeiling.CreateObject( new Vector3(x + _plane.MinCoordinat,_plane.LimitHeight + _HeightProection,z + +_plane.MinCoordinat) ));
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
