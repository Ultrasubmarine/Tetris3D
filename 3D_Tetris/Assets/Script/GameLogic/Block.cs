using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct CoordinatXZ {
    public int x;
    public int z;

    public CoordinatXZ( int x, int z) {
        this.x = x;
        this.z = z;
    }
}

public class Block : MonoBehaviour {
    public int x;
    public int y;
    public int z;

    public bool IsDestroy { get; set; }
    public Transform MyTransform;

    public MeshRenderer Mesh { get; private set; }
    
    public CoordinatXZ XZ { get { return new CoordinatXZ(x, z); } }

    private void Awake() {
        MyTransform = this.transform;
        Mesh = GetComponent<MeshRenderer>();
    }

    public Block() {
    }

    public Block(int xx, int yy, int zz) {
        x = xx;
        y = yy;
        z = zz;
    }

    public void SetCoordinat(Vector3 point) {
        x = (int) point.x - 1; // координаты plane - круговые
        y = (int) point.y;
        z = (int) point.z - 1; // координаты plane - круговые
    }

    void OnDisable()
    {
        IsDestroy = false;
        MyTransform.position = Vector3.zero;
        MyTransform.rotation = Quaternion.identity;          
    }
}