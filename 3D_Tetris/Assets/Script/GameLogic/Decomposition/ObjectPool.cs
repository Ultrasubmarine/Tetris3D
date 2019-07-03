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
    public GameObject Gobj;

    public PoolContainer(T obj, GameObject g) {
        Object = obj;
        Active = false;
        Transform =obj.transform;
        Gobj = g;
    }

    public void SetActive(bool value) {
        Active = value;
        Gobj.SetActive(value);
       // Transform.gameObject.SetActive(value);
    }
}

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour {
    [SerializeField] protected GameObject Prefab;
	[Header("начальное заполнение пула")]
	[SerializeField] protected bool InitialInitialization;
	[SerializeField] protected int CountObject;

	List<PoolContainer<T>> Pool = new List<PoolContainer<T>>();

    private void Awake()
    {
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

        var returnObj = Pool.FirstOrDefault(obj => !obj.Gobj.active);
        if( returnObj == null) {
            InstantiateObject();
            returnObj = Pool[Pool.Count - 1];
        }

        returnObj.SetActive(true);
        returnObj.Transform.position = position;
        return Pool[Pool.Count - 1].Object;
	}

	public void DestroyObject( GameObject obj) {

        var returnContainer = Pool.Find(s => s.Object == obj);
        returnContainer.SetActive(false);
    } 

	private void InstantiateObject() {

        GameObject GGG = Instantiate(Prefab);
        Debug.Log("Instn");
        PoolContainer<T> newObj = new PoolContainer<T>(GGG.GetComponent<T>(), GGG);
        newObj.SetActive(false);
        Pool.Add(newObj);
	}
}
