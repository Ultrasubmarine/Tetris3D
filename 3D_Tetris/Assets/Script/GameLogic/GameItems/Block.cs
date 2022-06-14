using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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

public enum BlockType
{
    simple,
    box,
    stone,
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

    public BlockType blockType;
    
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
        // myTransform.localScale = Vector3.one * 0.97f;
        
        isStar = false;
      //  _meshFilter.mesh = _meshCube;

        _star.transform.localPosition = Vector3.zero;
        _star.SetActive(false);
        _star.transform.DOKill();
        oreol.gameObject.SetActive(false);

        blockType = BlockType.simple;
    }

    public void TransformToStar(Mesh _starMesh, Material _starMaterial, Material _blockMaterial)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        _extraMeshFilter.mesh = _starMesh;
        extraMesh.material = _starMaterial;
        
       // Vector3.
        _star.layer = 11;//"element";
        _star.SetActive(true);

        if (blockType == BlockType.stone)
        {
            _star.transform.localPosition = new  Vector3(0, -0.1f, 0);
        }
      
        _star.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);
        
        oreol.gameObject.SetActive(true);
        
        oreol.localPosition = Vector3.zero;
        oreol.transform.localEulerAngles = Vector3.zero;
    }

    public void TransformToBomb(Mesh _bombMesh, Material _bombMaterial, Material _blockMaterial, Vector3 _rotation, bool isBig)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        _extraMeshFilter.mesh = _bombMesh;
        extraMesh.material = _bombMaterial;

        _star.layer = 15;//"Outlined";
        _star.SetActive(true);
        
        if(isBig)
            _star.transform.DOScale(Vector3.one, 0.8f).From(Vector3.one * 1.4f).SetLoops(-1,LoopType.Yoyo);
        else
            _star.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

        _star.transform.localEulerAngles = _rotation;
        
        oreol.transform.localEulerAngles = Vector3.zero;
        oreol.localPosition = Vector3.zero;
        // oreol.gameObject.SetActive(true);
    }
    
    public void TransformToBox(Mesh _boxMesh, Material _boxMaterial, Material _blockMaterial, Vector3 _rotation)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        _extraMeshFilter.mesh = _boxMesh;
        extraMesh.material = _boxMaterial;

        _star.layer = 11;//"element";
        _star.SetActive(true);
        
        _star.transform.DOScale(new Vector3(0.92f, 0.92f, 0.92f), 1f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

        _star.transform.localEulerAngles = Vector3.zero;
        
        oreol.gameObject.SetActive(true);
        oreol.transform.localEulerAngles = new Vector3(90, 0, 0);
        oreol.localPosition = new Vector3(0,0.1f,0f);
       // _star.transform.localEulerAngles = _rotation;
        // oreol.gameObject.SetActive(true);
        
        blockType = BlockType.box;
    }

    public void TransformToStone(Material _blockMaterial)
    {
        mesh.material = _blockMaterial;
        
        blockType = BlockType.stone;
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