using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using Boo.Lang.Environments;

public class PoolContainer
{
	public bool Active;
	public GameObject Object;


	public PoolContainer( GameObject obj) {
		Object = obj;
		Active = false;
	}

    public void SetActive( bool value)
    {
        Active = value;
        Object.SetActive(value);
    }
}

public class ObjectPool : MonoBehaviour
{	
	[SerializeField] GameObject Prefab;
	
	[Header("начальное заполнение пула")]
	[SerializeField] bool InitialInitialization;
	[SerializeField] int CountObject;

	List<PoolContainer> Pool = new List<PoolContainer>();

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

	public GameObject CreateObject() {

		int index = -1;
		
		for (int i = 0; i < Pool.Count; i++) {
			if (!Pool[i].Active) {
                Pool[i].SetActive(true);
                return Pool[i].Object;
            }
		}

		CreateObject();
        Pool[Pool.Count - 1].SetActive(true);
        return Pool[Pool.Count - 1].Object;
	}

	public void DestroyObject( GameObject obj) {

        var returnContainer = Pool.Find(s => s.Object == obj);
        returnContainer.SetActive(false);
    } 

	private void InstantiateObject() { 
		PoolContainer tmp = new PoolContainer( Instantiate(Prefab) );
        tmp.Object.gameObject.SetActive(false);
        Pool.Add(tmp);
	}
}
