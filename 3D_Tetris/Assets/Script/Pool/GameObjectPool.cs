using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameObjectPool : MonoBehaviour {

	[SerializeField] protected GameObject Prefab;
	[Header("начальное заполнение пула")]
	[SerializeField] protected bool InitialInitialization;
	[SerializeField] protected int CountObject;

	List<GameObject> Pool = new List<GameObject>();
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

	public GameObject CreateObject( Vector3 position) {

		var returnObj = Pool.FirstOrDefault(obj => !obj.active);
		if( returnObj == null)
		{
			InstantiateObject();
			returnObj = Pool[Pool.Count - 1];
		}

		returnObj.SetActive(true);
		returnObj.transform.position = position;
		return returnObj;
	}

	public void DestroyObject( GameObject obj) {

		var returnContainer = Pool.FirstOrDefault(s => s == obj);
		if (returnContainer == null)
			return;
        
		returnContainer.SetActive(false);
		returnContainer.transform.parent = this.transform;
	} 

	private void InstantiateObject() {
		GameObject newPoolObj = Instantiate(Prefab);
		newPoolObj.SetActive(false);
		Pool.Add(newPoolObj);
		newPoolObj.transform.parent = _transform;
	}
}
