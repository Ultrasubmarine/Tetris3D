using UnityEngine;

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
  //  public Vector3Int xyz;
    public Vector3Int coordinates => _coordinates;
    
    public Vector3Int _coordinates;
    public CoordinatXZ xz => new CoordinatXZ(coordinates.x, coordinates.z);

    public bool isDestroy { get; set; }

    public Transform myTransform { get; private set; }
    public MeshRenderer mesh { get; private set; }

    private void Awake()
    {
        myTransform = transform;
        mesh = GetComponent<MeshRenderer>();
    }

    public Block()
    {
    }

    public Block(int x, int y, int z)
    {
        _coordinates = new Vector3Int(x, y, z);
 //       xyz = _coordinates;
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
        isDestroy = false;
        myTransform.position = Vector3.zero;
        myTransform.rotation = Quaternion.identity;
    }
}