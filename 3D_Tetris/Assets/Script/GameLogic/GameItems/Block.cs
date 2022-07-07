using System;
using System.Numerics;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
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

    public int lives { get; private set; } // for stone blocks only 
    
    public Transform myTransform { get; private set; }
    public MeshRenderer mesh { get; private set; }
    public MeshRenderer extraMesh { get; private set; }
    
    public MeshFilter meshFilter { get; private set; }
    public MeshFilter extraMeshFilter { get; private set; }
    
    private static Mesh _meshCube;
    
    public bool isStar { get; private set; }

    public Action<Block> OnCollected;

    public Action<Block> OnDestroyed;
    
    public Action<Block> OnDamaged; // for stone
    
    public Transform oreol;
    
    [SerializeField] private GameObject _star;
    [SerializeField] private Outline _outline;
    
    public Transform Star => _star.transform;

    public BlockType blockType;
    
    private void Awake()
    {
        myTransform = transform;
        mesh = GetComponent<MeshRenderer>();
        extraMesh = _star.GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        extraMeshFilter = _star.GetComponent<MeshFilter>();
        
        if (_meshCube == null)
            _meshCube = meshFilter.mesh;
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
        _star.transform.localScale = Vector3.one;
        _star.transform.rotation = Quaternion.identity;
        _star.SetActive(false);
        _star.transform.DOKill();
        oreol.gameObject.SetActive(false);

        mesh.enabled = true;
        extraMesh.enabled = true;
        
        _outline.enabled = false;
        mesh.enabled = true;
        
        blockType = BlockType.simple;
    }

    public void Outline(bool isOn)
    {
        _outline.enabled = isOn;
        
        if (blockType == BlockType.box)
        {
            mesh.enabled = isOn;
        }
    }
    
    public void Hide()
    {
        mesh.enabled = false;
        extraMesh.enabled = false;
        oreol.gameObject.SetActive(false);
    }
    
    public void TransformToStar(Mesh _starMesh, Material _starMaterial, Material _blockMaterial)
    {
        isStar = true;
        
        mesh.material = _blockMaterial;
        
        extraMeshFilter.mesh = _starMesh;
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
    //    isStar = true;
        
        mesh.material = _blockMaterial;
        
        extraMeshFilter.mesh = _bombMesh;
        extraMesh.material = _bombMaterial;

        _star.layer = 15;//"Outlined";
        _star.SetActive(true);
        
        if(isBig)
            _star.transform.DOScale(Vector3.one, 0.8f).From(Vector3.one * 1.1f).SetLoops(-1,LoopType.Yoyo);
        else
            _star.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

        _star.transform.localEulerAngles = _rotation;
        
        oreol.transform.localEulerAngles = Vector3.zero;
        oreol.localPosition = Vector3.zero;
        // oreol.gameObject.SetActive(true);
    }
    
    public void TransformToBox(Mesh _boxMesh, Material _boxMaterial, Material _blockMaterial, Vector3 _rotation)
    {
     //   isStar = true;

        mesh.enabled = false;
        mesh.material = _blockMaterial;
        
        extraMeshFilter.mesh = _boxMesh;
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

    public void TransformToStone(Mesh _cellMesh, Material _cellMaterial, Material blockMaterial, int lives)
    {
        _star.transform.rotation = Quaternion.identity;
        _star.layer = 11;//"element";
        _star.SetActive(true);
        
        extraMeshFilter.mesh = _cellMesh;
        extraMesh.material = _cellMaterial;

        this.lives = lives;
        mesh.material = blockMaterial;
        blockType = BlockType.stone;
    }
    public void Collect()
    {
        OnCollected?.Invoke(this);
        //if (isStar) 
        // animation TO DO
    }

    /// returned int - amount of lives after decrease;
    public int DecreaseLives()
    {
        --lives;
        OnDamaged?.Invoke(this);
        return lives;
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