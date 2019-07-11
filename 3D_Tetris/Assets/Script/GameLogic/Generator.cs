using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;

public class Generator : MonoBehaviour {

    [Header("Pools")]
    [SerializeField] ElementPool _ElementPool;
    [SerializeField] BlockPool _BlockPool;
    [Space(15)]
    
    [SerializeField] Material[] _MyMaterial;
    [Tooltip(" подсказка места расположения падающего элемента")]
    [SerializeField] Material _BonusMaterial;

    PlaneMatrix _matrix;
    bool[,,] _castMatrix;
    Vector3Int _minPoint;

    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _castMatrix = new bool[3, 3, 3];
    }
    
    public Element GenerationNewElement( Transform elementParent){

        _minPoint = _matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(_minPoint.y);

        Element newElement = GenerateElement();

        Vector3 pos = elementParent.position;
        newElement.InitializationAfterGeneric(_matrix.Height);

        // выравниваем элемент относительно координат y 
        var min_y = newElement.MyBlocks.Min(s => s.y);
        var max_y = newElement.MyBlocks.Max(s => s.y);

        int size = max_y - min_y;
        newElement.MyTransform.position = new Vector3(pos.x, pos.y + _matrix.Height - size, pos.z);

        CreateDuplicate(newElement);
      //  ChengeBlock(Element, plane.gameObject);
        return newElement;
    }

    private Element GenerateElement() {

        int indexMat = Random.Range(0, _MyMaterial.Length - 1);

        Element createElement = _ElementPool.CreateObject(Vector3.zero);
        
        Vector3Int lastPoint = new Vector3Int(_minPoint.x, 0, _minPoint.z);
        _castMatrix[ _minPoint.x, 0, _minPoint.z] = false;
        
        CreateBlock(lastPoint, createElement, indexMat);
       
        List<Vector3Int> freePlaces; 
        for (int i = 0; i < 3; i++) {
            freePlaces = FoundFreePlacesAround(lastPoint);
            lastPoint = freePlaces[ Random.Range(0, freePlaces.Count-1) ];

            CreateBlock(lastPoint, createElement, indexMat);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;       
        }
        return createElement;
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
    
    private void CreateBlock(Vector3 position, Element element, int indexMat)
    {
        Block currBlock = _BlockPool.CreateObject(Vector3Int.zero);
        currBlock.Mesh.material = _MyMaterial[indexMat];
        currBlock.SetCoordinat(position);
       
        currBlock.MyTransform.parent = element.gameObject.transform;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);     
    }

    private void SetBlockPosition(Block block) {
        Vector3 position = new Vector3(block.x, block.y, block.z);
        block.gameObject.transform.localPosition = position;
    }

    // TODO Duplicate 
    private void CreateDuplicate( Element element)
    {
//        _Duplicate = _ElementPool.CreateObject(Vector3Int.zero);
//        
//        foreach (var item in element.MyBlocks) {
//            
//            item.GetComponent<Renderer>().material = _BonusMaterial;
//            item.gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
//        }

//last --->
//        // TO DO - вычленить в отдельный метод создание дубляжа
//        GameObject exElement = Instantiate(newElement);
//        exElement.name = "TUTOR";
//        foreach (var item in exElement.GetComponent<Element>().MyBlocks) {
//            item.GetComponent<Renderer>().material = _BonusMaterial;
//            item.gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
//        }
//
//        exElement.transform.position =
//            new Vector3(temp.x, elementParent.gameObject.transform.position.y + _minYforElement, temp.z);
////        examleElement = exElement; // Destroy(exElement, 5f);
//        ////////////
    }

    public Element CreateEmptyElement()
    {
        return _ElementPool.CreateObject(Vector3.zero);
    }
    
    public void DeleteBlock(Block block) {
        _BlockPool.DestroyObject(block);
    }

    public void DeleteElement(Element element) {
        _ElementPool.DestroyObject(element);
    }
//    void ChengeBlock(Element element, GameObject target) {
//        Random rn = new Random();
//
//        int turnCount = Random.Range(1, 2);
//        if (turnCount > 0) {
//            turn direction = (turn) Random.Range(0, 1 + 1);
//            Debug.Log(direction.ToString());
//            while (turnCount > 0) {
//                element.SetTurn(direction, target);
//                turnCount--;
//            }
//        }
//
//        int moveCount = Random.Range(0, 2);
//        if (moveCount > 0) {
//            move directionMove = (move) Random.Range(0, 4 + 1);
//            while (turnCount > 0) {
//                if (element.CheckMove(directionMove, 3 / 2 * (-1))) {
//                    element.SetMove(directionMove);
//                    moveCount--;
//                }
//                else
//                    break;
//            }
//        }
//    }
}