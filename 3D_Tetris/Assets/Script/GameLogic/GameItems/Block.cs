using System;
using DG.Tweening;
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

    public static bool operator ==  (CoordinatXZ one, CoordinatXZ two)
    {
        if (one.x != two.x || one.z != two.z)
            return false;
        return true;
    }
    
    public static bool operator !=  (CoordinatXZ one, CoordinatXZ two)
    {
        if (one.x != two.x || one.z != two.z)
            return true;
        return false;
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
    public MeshRenderer extraMesh { get; private set; }
    
    MeshFilter _meshFilter;
    MeshFilter _extraMeshFilter;
    
    private static Mesh _meshCube;
    
    public bool isStar { get; private set; }

    public Action<Block> OnCollected;

    public Action<Block> OnDestroyed;
    
    public Transform oreol;
    
    [SerializeField] private GameObject _star;
    public Transform Star => _star.transform;
    
    private void Awake()
    {
        myTransform = transform;
        mesh = GetComponent<MeshRenderer>();
        extraMesh = _star.GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _extraMeshFilter = _star.GetComponent<MeshFilter>();
        
        if (_meshCube == null)
            _meshCube = _meshFilter.mesh;
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
        isStar = false;
      //  _meshFilter.mesh = _meshCube;
        
        _star.SetActive(false);
        _star.transform.DOKill();
        oreol.gameObject.SetActive(false);
    }

    public void TransformToStar(Mesh _starMesh, Material _starMaterial, Material _blockMaterial)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        _extraMeshFilter.mesh = _starMesh;
        extraMesh.material = _starMaterial;
        
        _star.layer = 11;//"element";
        _star.SetActive(true);
        _star.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);
        oreol.gameObject.SetActive(true);
    }

    public void TransformToBomb(Mesh _bombMesh, Material _bombMaterial, Material _blockMaterial, Vector3 _rotation)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        _extraMeshFilter.mesh = _bombMesh;
        extraMesh.material = _bombMaterial;

        _star.layer = 15;//"Outlined";
        _star.SetActive(true);
        _star.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

        _star.transform.localEulerAngles = _rotation;
        // oreol.gameObject.SetActive(true);
    }
    
    public void Collect()
    {
        OnCollected?.Invoke(this);
        //if (isStar) 
        // animation TO DO
    }

    public void Destroy() // for bombs
    {
        OnDestroyed?.Invoke(this);
    }
    // FOR override & pickable blocks
    public virtual bool IsPickable()
    {
        return false;
    }

    public virtual void Pick(Element element)
    {
    }
}