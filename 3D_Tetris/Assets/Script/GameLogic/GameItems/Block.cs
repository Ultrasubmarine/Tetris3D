using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct CoordinatXZ
{
    public int x;
    public int z;

    public CoordinatXZ(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public class Block : MonoBehaviour
{
    public Vector3Int Coordinates => _coordinates;
    private Vector3Int _coordinates;
    public CoordinatXZ XZ => new CoordinatXZ(Coordinates.x, Coordinates.z);

    public bool IsDestroy { get; set; }

    public Transform MyTransform { get; private set; }
    public MeshRenderer Mesh { get; private set; }

    private void Awake()
    {
        MyTransform = transform;
        Mesh = GetComponent<MeshRenderer>();
    }

    public Block()
    {
    }

    public Block(int x, int y, int z)
    {
        _coordinates = new Vector3Int(x, y, z);
    }

    public void SetCoordinates(int x, int y, int z)
    {
        _coordinates.x = x;
        _coordinates.y = y;
        _coordinates.z = z;
    }

    public void OffsetCoordinates(int x = 0, int y = 0, int z = 0)
    {
        _coordinates.x += x;
        _coordinates.y += y;
        _coordinates.z += z;
    }

    private void OnDisable()
    {
        IsDestroy = false;
        MyTransform.position = Vector3.zero;
        MyTransform.rotation = Quaternion.identity;
    }
}