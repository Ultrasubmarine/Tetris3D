using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockScript : MonoBehaviour {

    public int x;
    public int y;
    public int z;

    public bool destroy = false ;

    public BlockScript()
    {

    }

    public BlockScript( int xx, int yy, int zz)
    {
        x = xx;
        y = yy;
        z = zz;
    }

    public void SetCoordinat(Vector3 point)
    {
        x = (int)point.x - 1; // координаты plane - круговые
        y = (int)point.y;
        z = (int)point.z - 1; // координаты plane - круговые
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}
}
