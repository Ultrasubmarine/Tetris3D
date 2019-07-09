using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;
using UnityEngine.Serialization;

//public class Element {
//    public GameObject El;
//    public double P;
//}

public class Generator : MonoBehaviour {

    [FormerlySerializedAs("prefabBlock")] [FormerlySerializedAs("PrefabBlock")] public GameObject _PrefabBlock;

    [FormerlySerializedAs("MyMaterial")] [SerializeField] Material[] _MyMaterial;

    [SerializeField] Material _BonusMaterial;
    int _minYforElement;
    [FormerlySerializedAs("ExamleElement")] public GameObject _ExamleElement;

    [SerializeField] PlaneMatrix _Matrix;
    bool[,,] _castMatrix;

    [SerializeField] ElementPool _ElementPool;
    
    private void Start() {
        _castMatrix = new bool[3, 3, 3];
    }
    public GameObject GenerationNewElement( Transform elementParent){

        Vector3 min = _Matrix.FindLowerAccessiblePlace();
        _minYforElement =(int) min.y;
        bool[,,] matrixCheck = CreateCastMatrix((int)min.y);

        GameObject newElement = CreateElement();

        //устанавливаем нормальную позицию элемента
        Vector3 temp = elementParent.position;

        // инициализируем блоки элемента согласно установленной позиции
        Element element = newElement.GetComponent<Element>();
        element.InitializationAfterGeneric(_Matrix.Height);//plane.Height);

        //// выравниваем элемент относительно координат y 
        var min_y = element.MyBlocks.Min(s => s.y);
        var max_y = element.MyBlocks.Max(s => s.y);

        int size = max_y - min_y;

        newElement.transform.position = new Vector3(temp.x, temp.y + _Matrix.Height - size, temp.z);

//        // TO DO - вычленить в отдельный метод создание дубляжа
//        GameObject exElement = Instantiate(newElement);
//        exElement.name = " TUTOR";
//        foreach (var item in exElement.GetComponent<Element>().MyBlocks) {
//            item.GetComponent<Renderer>().material = _BonusMaterial;
//            item.gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
//        }
//
//        exElement.transform.position =
//            new Vector3(temp.x, elementParent.gameObject.transform.position.y + _minYforElement, temp.z);
////        examleElement = exElement; // Destroy(exElement, 5f);
//        ////////////
//
//      //  ChengeBlock(Element, plane.gameObject);
        return newElement;
    }

    private GameObject CreateElement() {

        int indexMat = Random.Range(0, _MyMaterial.Length - 1);
        
        Vector3Int min = _Matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(min.y);

        Element createElement = _ElementPool.CreateObject(Vector3.zero);
        
        Vector3Int lastPoint = new Vector3Int(min.x, 0, min.z);
        _castMatrix[ min.x, 0, min.z] = false;
        
        CreateBlock(lastPoint, createElement, indexMat);
       
        List<Vector3Int> freePlaces; 
        for (int i = 0; i < 3; i++) {
            freePlaces = FoundFreePlacesAround(lastPoint);
            lastPoint = freePlaces[ Random.Range(0, freePlaces.Count-1) ];

            CreateBlock(lastPoint, createElement, indexMat);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;       
        }
        return createElement.gameObject;
    }

    private bool[,,] CreateCastMatrix(int min) {

        bool[,,] castMatrix = new bool[3, 3, 3];
        int barrier;

        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                barrier = _Matrix.MinHeightInCoordinates(x,z);
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
    
    private void CreateBlock(Vector3 position, Element element, int indexMat) {

        GameObject currBlock = Instantiate(_PrefabBlock);
        currBlock.AddComponent<Block>().SetCoordinat(position);
        currBlock.GetComponent<MeshRenderer>().material = _MyMaterial[indexMat];

        currBlock.gameObject.transform.parent = element.gameObject.transform;
        currBlock.transform.localPosition = position;
        SetBlockPosition(currBlock.GetComponent<Block>());
        element.AddBlock(currBlock.GetComponent<Block>());
    }

    private void SetBlockPosition(Block block) {
        Vector3 position = new Vector3(block.x, block.y, block.z);
        block.gameObject.transform.localPosition = position;
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