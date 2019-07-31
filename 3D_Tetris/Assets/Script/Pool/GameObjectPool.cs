using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

public class GameObjectPool : MonoBehaviour {

	[SerializeField] protected GameObject _Prefab;
	[Header("начальное заполнение пула")]
	[SerializeField] protected bool _InitialInitialization;
	[SerializeField] protected int _CountObject;

	List<GameObject> _pool = new List<GameObject>();
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

	public GameObject CreateObject( Vector3 position) {

		var returnObj = _pool.FirstOrDefault(obj => !obj.active);
		if( returnObj == null)
		{
			InstantiateObject();
			returnObj = _pool[_pool.Count - 1];
		}

		returnObj.SetActive(true);
		returnObj.transform.position = position;
		return returnObj;
	}

	public void DestroyObject( GameObject obj) {

		var returnContainer = _pool.FirstOrDefault(s => s == obj);
		if (returnContainer == null)
			return;
        
		returnContainer.SetActive(false);
		returnContainer.transform.parent = this.transform;
	} 

	private void InstantiateObject() {
		GameObject newPoolObj = Instantiate(_Prefab);
		newPoolObj.SetActive(false);
		_pool.Add(newPoolObj);
		newPoolObj.transform.parent = _transform;
	}
}
