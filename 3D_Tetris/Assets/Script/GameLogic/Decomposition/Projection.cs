using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour
{

    PlaneSize _planeSize;
    // TO DO - статическая переменная - высота сцены.
  //  int _HeightPlane;

    public const int PROECTIONS = 1;
    public const int CEILING = 2;

    [Header(" Проекция ")]
    [SerializeField] ObjectPool _PoolProjection;
    [SerializeField] float _HeightProection = 0.1f;
    private List<GameObject> _proectionsList;

    [Header(" Потолок ")] // ceiling - потолок
    [SerializeField] ObjectPool _PoolСeiling;
    private List<GameObject> _ceilingList; 
    
    // ФУНКЦИИ ДЛЯ РАБОТЫ С ПРОЕКЦИЯМИ
    private void CreateProjection(ElementScript obj)
    {
        bool flagCreate;

        Destroy(PROECTIONS); 

        foreach (var item in obj.MyBlocks)
        {
            flagCreate = false;

            foreach (var proectionItem in _proectionsList) // проверяем есть ли уже проекция на эту клетку поля
            {
                if (proectionItem.gameObject.transform.position.x == item.x && proectionItem.gameObject.transform.position.z == item.z)
                    flagCreate = true; // мы уже такую клетку прописали
            }

            if (!flagCreate)
            {
                GameObject tmp = _PoolProjection.CreateObject();

                float y = -1;
                for (int i = PlaneSize.Height - 1; i > -1; i--)
                {
                    if (_planeSize._block[(int)item.x + 1, i, (int)item.z + 1] != null)
                    {
                        y = i;
                        break;
                    }
                }

                tmp.transform.position = new Vector3(item.x, (y + 1.0f + _HeightProection), item.z);
                _proectionsList.Add(tmp);
            }
        }
    }


    public void Destroy(int typeObject /* const PROECTIONS or CEILING*/ )
    {
        List<GameObject> list;
        ObjectPool pool;

        switch (typeObject)
        {
            case PROECTIONS:
                {
                    list = _proectionsList;
                    pool = _PoolProjection;
                    break;
                }
            case CEILING:
                {
                    list = _ceilingList;
                    pool = _PoolСeiling;
                    break;
                }
            default:
                {
                    Debug.Log("ERROR: value proections not found (Projection.cs)");
                    return;
                    break;
                }
        }
        DestroyList(list, pool);
    }

    private void DestroyList( List<GameObject> list, ObjectPool pool )
    {
        GameObject tmpDestroy;
        foreach (var item in list)
        {
            tmpDestroy = item;
            pool.DestroyObject(tmpDestroy);
        }

        list.Clear();
    }

    private void CreateCeiling( PlaneScript plane)
    {
        Destroy(CEILING);

        for (int x = 0; x < plane.Wight; x++) // подумать насчет 3-ки
        {
            for (int z = 0; z < plane.Wight; z++)
            {
                if (plane._block[x, (int)(plane.LimitHeight - 1), z] != null || plane._block[x, (int)(plane.LimitHeight - 2), z] != null)
                {
//                    GameObject tmp = _;
//
//                    tmp.transform.position = new Vector3(x - 1, (_LimitHeight + HeightProection), z - 1);
//                    _potolocList.Add(tmp);
                }
            }
        }
    }
}
