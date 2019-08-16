using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using IntegerExtension;

public class Generator : MonoBehaviour {

    [Header("Pools")]
    [SerializeField] ElementPool _ElementPool;
    [SerializeField] BlockPool _BlockPool;
    [Space(15)]
    
    [SerializeField] Material[] _MyMaterial;
    [Tooltip(" подсказка места расположения падающего элемента")]
    [SerializeField] Material _BonusMaterial;

    [Header("For turn & move")] 
    [SerializeField] Moving _Mover;
    [SerializeField] Turning _Turner;
    
    PlaneMatrix _matrix;
    bool[,,] _castMatrix;
    Vector3Int _minPoint;

    Element _answerElement;
    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _castMatrix = new bool[3, 3, 3];
    }
    
    public Element GenerationNewElement( Transform elementParent){

        _minPoint = _matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(_minPoint.y);

        Element newElement = GenerateElement();
        
//       CreateDuplicate(newElement);
        
        Vector3 pos = elementParent.position;
        newElement.InitializationAfterGeneric(_matrix.Height);
        
        // выравниваем элемент относительно координат y 
        var min_y = newElement.MyBlocks.Min(s => s.Coordinates.y);
        var max_y = newElement.MyBlocks.Max(s => s.Coordinates.y);

        int size = max_y - min_y;
        newElement.MyTransform.position = new Vector3(pos.x, pos.y + _matrix.Height - size, pos.z);

        //ConfuseElement(newElement);//, plane.gameObject);
        return newElement;
    }

    private bool[,,] CreateCastMatrix(int min) {

        bool[,,] castMatrix = new bool[3, 3, 3];
        int barrier;

        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                barrier = _matrix.MinHeightInCoordinates(x,z);
                for (int y = min + 3 - 1; y >= min; y--) {
                    castMatrix[x, y - min, z] = y < barrier ? false : true ;               
                }
            }
        }
        return castMatrix;
    }
    
    private Element GenerateElement() {

        int indexMat = Random.Range(0, _MyMaterial.Length - 1);

        Element createElement = _ElementPool.CreateObject(Vector3.zero);
        
        Vector3Int lastPoint = new Vector3Int(_minPoint.x, 0, _minPoint.z);
        _castMatrix[ _minPoint.x, 0, _minPoint.z] = false;
        
        CreateBlock(lastPoint, createElement, indexMat);
        Debug.ClearDeveloperConsole();
        List<Vector3Int> freePlaces;
        int index;
        for (int i = 0; i < 3; i++) {
            freePlaces = FoundFreePlacesAround(lastPoint);
            if( freePlaces.Count == 0)
                break;
            lastPoint = freePlaces[  Random.Range(0, freePlaces.Count) ];

            CreateBlock(lastPoint, createElement, indexMat);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;       
        }
        return createElement;
    }
    
    private void CreateBlock(Vector3Int position, Element element, int indexMat)
    {
        Block currBlock = _BlockPool.CreateObject(Vector3Int.zero);
        currBlock.Mesh.material = indexMat < 100 ? _MyMaterial[indexMat] : _BonusMaterial;
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat() );
       
        currBlock.MyTransform.parent = element.gameObject.transform;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);     
    }
    
    private void SetBlockPosition(Block block) {
        Vector3 position = new Vector3(block.Coordinates.x, block.Coordinates.y, block.Coordinates.z);
        block.gameObject.transform.localPosition = position;
    }
    
    private List<Vector3Int> FoundFreePlacesAround(Vector3Int point) {
        List<Vector3Int> listPov = new List<Vector3Int>();

        if( CheckEmptyPlace(point + new Vector3Int(1, 0, 0)) )
            listPov.Add(point + new Vector3Int(1, 0, 0));

        if (CheckEmptyPlace(point + new Vector3Int(-1, 0, 0)))
            listPov.Add(point + new Vector3Int(-1, 0, 0));

        if (CheckEmptyPlace(point + new Vector3Int(0, 0, 1)))
            listPov.Add(point + new Vector3Int(0, 0, 1));

        if (CheckEmptyPlace(point + new Vector3Int(0, 0, -1)))
            listPov.Add(point + new Vector3Int(0, 0, -1));

        if (point.y < 2)
            if (CheckEmptyPlace(point + new Vector3Int(0, 1, 0)))
                listPov.Add(point + new Vector3Int(0, 1, 0));

        return listPov;
    }

    private bool CheckEmptyPlace( Vector3Int indices) {
        if (indices.OutOfIndexLimit())
            return false;

        if (indices.y != 0 && _castMatrix[indices.x, indices.y - 1, indices.z])
            return false;

        return _castMatrix[indices.x, indices.y, indices.z];
    }

    private void CreateDuplicate( Element element)
    {
        if( !ReferenceEquals( _answerElement, null) )
            DestroyOldDuplicate();
        _answerElement= _ElementPool.CreateObject(Vector3Int.zero);

        Vector3Int stabiliation = new Vector3Int(1, 0, 1);
        foreach (var item in element.MyBlocks)
        {
            //CreateBlock( ( (Vector3Int)item.MyTransform.position + stabiliation), _answerElement, 666);
        }
       _answerElement.MyTransform.position += new Vector3(0,0.42f, 0); 
    }

    private void DestroyOldDuplicate()
    {
        foreach (var block in _answerElement.MyBlocks)
        {
            DeleteBlock(block);
        }
        DeleteElement(_answerElement);
    }

    public Element CreateEmptyElement()
    {
        return _ElementPool.CreateObject(Vector3.zero);
    }
    
    public void DeleteBlock(Block block) {
        _BlockPool.DestroyObject(block);
    }

    public void DeleteElement(Element element) {
        if (element.MyBlocks.Count > 0) {
            foreach (var block in element.MyBlocks) {
                DeleteBlock(block);
                Debug.Log("Delete Block IN ");
            }
        }
        _ElementPool.DestroyObject(element);
    }
    
    void ConfuseElement(Element element){//, GameObject target) {
        Random rn = new Random();

          int turnCount = Random.Range(1, 2);
//        if (turnCount > 0) {
//            turn direction = (turn) Random.Range(0, 1 + 1);
//            Debug.Log(direction.ToString());
//            while (turnCount > 0) {
//                element.SetTurn(direction, target);
//                turnCount--;
//            }
//        }

        int moveCount = Random.Range(0, 2);
        if (moveCount > 0) {
            move directionMove = (move) Random.Range(0, 4 + 1);
            while (moveCount > 0) {
                if ( _Mover.MomentaryActionForGenerator(element, directionMove)) {
                    Debug.Log("mover in " + directionMove.ToString());
                    moveCount--;
                }
                else
                    break;
            }
        }
    }
}