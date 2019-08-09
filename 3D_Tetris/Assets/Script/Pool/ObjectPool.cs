using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using Boo.Lang.Environments;
using UnityEngine.Serialization;

public class PoolContainer<T> where T : MonoBehaviour {
    public bool Active;
    public T Object;
    public Transform Transform;
    public GameObject GObj;

    public PoolContainer(T obj, GameObject g) {
        Object = obj;
        Active = false;
        Transform =obj.transform;
        GObj = g;
    }

    public void SetActive(bool value) {
        Active = value;
        GObj.SetActive(value);
    }
}

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour {
    [SerializeField] protected GameObject _Prefab;
    [Header("начальное заполнение пула")]
	[SerializeField] protected bool _InitialInitialization;
	[SerializeField] protected int _CountObject;

	List<PoolContainer<T>> _pool = new List<PoolContainer<T>>();
    private Transform _transform;
    
    private void Awake()
    {
        _transform = this.gameObject.transform;
        if(_InitialInitialization)
        {
            FirstInitialization();
        }
    }

    public void FirstInitialization() {
        
        for (int i = 0; i < _CountObject; i++)
        {
            InstantiateObject();
        }
    }

    public T CreateObject( Vector3 position) {

        var returnObj = _pool.FirstOrDefault(obj => !obj.GObj.active);
        if( ReferenceEquals(returnObj, null))
        {
            InstantiateObject();
            returnObj = _pool[_pool.Count - 1];
        }

        returnObj.SetActive(true);
        returnObj.Transform.position = position;
        return returnObj.Object;
    }

	public void DestroyObject( T obj) {

        var returnContainer = _pool.FirstOrDefault(s => s.Object == obj);
        if (ReferenceEquals(returnContainer, null))
            return;
        
        returnContainer.SetActive(false);
        returnContainer.Transform.parent = this.transform;
    } 

	private void InstantiateObject() {

        GameObject newPoolObj = Instantiate(_Prefab);
        PoolContainer<T> newObj = new PoolContainer<T>(newPoolObj.GetComponent<T>(), newPoolObj);
        newObj.SetActive(false);
        _pool.Add(newObj);
        newPoolObj.transform.parent = _transform;

    }
}
