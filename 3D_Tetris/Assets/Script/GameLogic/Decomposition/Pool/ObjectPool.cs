using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using Boo.Lang.Environments;

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
       // Transform.gameObject.SetActive(value);
    }
}

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour {
    [SerializeField] protected GameObject Prefab;
	[Header("начальное заполнение пула")]
	[SerializeField] protected bool InitialInitialization;
	[SerializeField] protected int CountObject;

	List<PoolContainer<T>> Pool = new List<PoolContainer<T>>();
    private Transform _transform;
    
    private void Awake()
    {
        _transform = this.gameObject.transform;
        if(InitialInitialization)
        {
            FirstInicialization();
        }
    }

    public void FirstInicialization() {
        
        for (int i = 0; i < CountObject; i++)
        {
            InstantiateObject();
        }
    }

    public T CreateObject( Vector3 position) {

		int index = -1;

        var returnObj = Pool.FirstOrDefault(obj => !obj.GObj.active);
        if( returnObj == null)
        {
            InstantiateObject();
            Debug.Log("Create new obj / pull = null");
            returnObj = Pool[Pool.Count - 1];
        }

        returnObj.SetActive(true);
        returnObj.Transform.position = position;
        return returnObj.Object;
    }

	public void DestroyObject( T obj) {

        var returnContainer = Pool.FirstOrDefault(s => s.Object == obj);
        if (returnContainer == null)
            return;
        
        returnContainer.SetActive(false);
        returnContainer.Transform.parent = this.transform;
    } 

	private void InstantiateObject() {

        GameObject newPoolObj = Instantiate(Prefab);
        Debug.Log("Instn");
        PoolContainer<T> newObj = new PoolContainer<T>(newPoolObj.GetComponent<T>(), newPoolObj);
        newObj.SetActive(false);
        Pool.Add(newObj);
        newPoolObj.transform.parent = _transform;

    }
}
