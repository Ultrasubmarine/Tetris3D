using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wantObj : MonoBehaviour
{

	[SerializeField] private List<Element> MyList;

	[SerializeField] private ElementPool Pool;
	// Use this for initialization
	void Start () {
		MyList = new List<Element>();
	}

	public void Create()
	{
		var tmp = Pool.CreateObject(this.transform.position);
		MyList.Add(tmp);
		MyList[MyList.Count - 1].transform.parent = this.transform;
		Debug.Log("Push");
	}

	public void Destroy()
	{
		if (MyList.Count > 0)
		{
			Element tmp = MyList[0];
			MyList.Remove(tmp);
			MyList.Remove(tmp);
			Pool.DestroyObject(tmp);
			Debug.Log("Pop");
		}
	
	}
	
}
