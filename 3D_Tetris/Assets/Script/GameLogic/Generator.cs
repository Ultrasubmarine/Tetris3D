using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;

public class Generator : MonoBehaviour
{
    private GameLogicPool _pool;

    [SerializeField] private Material[] _MyMaterial;

    [Tooltip(" подсказка места расположения падающего элемента")] [SerializeField]
    private Material _BonusMaterial;

    private PlaneMatrix _matrix;
    private bool[,,] _castMatrix;
    private Vector3Int _minPoint;

    private Element _answerElement;

    private void Start()
    {
        _matrix = RealizationBox.Instance.matrix;
        _pool = RealizationBox.Instance.gameLogicPool;

        _castMatrix = new bool[3, 3, 3];
        
        _answerElement= _pool.CreateEmptyElement();
        _answerElement.myTransform.parent = transform;
        _answerElement.myTransform.position = new Vector3(0,0.42f, 0);
    }

    public Element GenerationNewElement(Transform elementParent)
    {
        _minPoint = _matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(_minPoint.y);

        var newElement = GenerateElement();
        CreateDuplicate(newElement);

        var pos = elementParent.position;
        newElement.InitializationAfterGeneric(_matrix.height);

        // выравниваем элемент относительно координат y 
        var min_y = newElement.blocks.Min(s => s.coordinates.y);
        var max_y = newElement.blocks.Max(s => s.coordinates.y);

        var size = max_y - min_y;
        newElement.myTransform.position = new Vector3(pos.x, pos.y + _matrix.height - size, pos.z);

        //ConfuseElement(newElement);//, plane.gameObject);
        return newElement;
    }

    private bool[,,] CreateCastMatrix(int min)
    {
        var castMatrix = new bool[3, 3, 3];
        int barrier;

        for (var x = 0; x < 3; x++)
        for (var z = 0; z < 3; z++)
        {
            barrier = _matrix.MinHeightInCoordinates(x, z);
            for (var y = min + 3 - 1; y >= min; y--) castMatrix[x, y - min, z] = y >= barrier;
        }

        return castMatrix;
    }

    private Element GenerateElement()
    {
        var indexMat = Random.Range(0, _MyMaterial.Length - 1);

        var createElement = _pool.CreateEmptyElement();

        var lastPoint = new Vector3Int(_minPoint.x, 0, _minPoint.z);
        _castMatrix[_minPoint.x, 0, _minPoint.z] = false;

        _pool.CreateBlock(lastPoint, createElement, _MyMaterial[indexMat]);

        List<Vector3Int> freePlaces;
        for (var i = 0; i < 3; i++)
        {
            freePlaces = FoundFreePlacesAround(lastPoint);
            if (freePlaces.Count == 0)
                break;
            lastPoint = freePlaces[Random.Range(0, freePlaces.Count)];

            _pool.CreateBlock(lastPoint, createElement, _MyMaterial[indexMat]);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;
        }

        return createElement;
    }

    private List<Vector3Int> FoundFreePlacesAround(Vector3Int point)
    {
        var listPov = new List<Vector3Int>();

        if (CheckEmptyPlace(point + new Vector3Int(1, 0, 0)))
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

    private bool CheckEmptyPlace(Vector3Int indices)
    {
        if (indices.OutOfIndexLimit())
            return false;

        if (indices.y != 0 && _castMatrix[indices.x, indices.y - 1, indices.z])
            return false;

        return _castMatrix[indices.x, indices.y, indices.z];
    }

    private void CreateDuplicate( Element element)
    {
        DestroyOldDuplicate();
        Vector3Int stabiliation = new Vector3Int(1, 0, 1);
        foreach (var item in element.blocks)
        {
            var position = item.myTransform.position;
            Vector3Int v = stabiliation + new Vector3Int((int) position.x, (int) position.y, (int) position.z);
            _pool.CreateBlock(v, _answerElement, _BonusMaterial);
        }

        var answerPosition = _answerElement.myTransform.position;
        answerPosition = new Vector3(answerPosition.x, 0.42f + _minPoint.y, answerPosition.z);
        _answerElement.myTransform.position = answerPosition;
    }

    private void DestroyOldDuplicate()
    {
        foreach (var block in _answerElement.blocks)
        {
            _pool.DeleteBlock(block);
        }
        _answerElement.RemoveBlocksInList(_answerElement.blocks.ToArray());
    }

//    void ConfuseElement(Element element){//, GameObject target) {
//        Random rn = new Random();
//
//          int turnCount = Random.Range(1, 2);
////        if (turnCount > 0) {
////            turn direction = (turn) Random.Range(0, 1 + 1);
////            Debug.Log(direction.ToString());
////            while (turnCount > 0) {
////                element.SetTurn(direction, target);
////                turnCount--;
////            }
////        }
//
//        int moveCount = Random.Range(0, 2);
//        if (moveCount > 0) {
//            move directionMove = (move) Random.Range(0, 4 + 1);
//            while (moveCount > 0) {
//                if ( _Mover.MomentaryActionForGenerator(element, directionMove)) {
//                    Debug.Log("mover in " + directionMove.ToString());
//                    moveCount--;
//                }
//                else
//                    break;
//            }
//        }
//    }
}