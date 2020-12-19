using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;
using Script.Controller;
using Script.Influence;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    private GameLogicPool _pool;

    [SerializeField] private Material[] _MyMaterial;
    [SerializeField] private GameObject _answerElementParent;

    [Space(10)] [Header("GD Balance height generate element")]
    [SerializeField] private int _minHeight;

    [Tooltip(" подсказка места расположения падающего элемента")] [SerializeField]
    private Material _BonusMaterial;

    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private GameCamera _gameCamera;
    
    private bool[,,] _castMatrix;
    private Vector3Int _minPoint;

    public Element _answerElement;

    private void Start()
    {
        _matrix = RealizationBox.Instance.matrix;
        _pool = RealizationBox.Instance.gameLogicPool;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _gameCamera = RealizationBox.Instance.gameCamera;

        _castMatrix = new bool[3, 3, 3];
        
        _answerElement= _pool.CreateEmptyElement();
        _answerElement.myTransform.parent = _answerElementParent.transform;
        _answerElement.myTransform.position = new Vector3(0,0.42f, 0);
        _answerElement.gameObject.SetActive(false);
    }

    public Element GenerationNewElement(Transform elementParent)
    {
        _minPoint = _matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(_minPoint.y);

        var newElement = GenerateElement();
        CreateDuplicate(newElement);

        var pos = elementParent.position;

        // выравниваем элемент относительно координат y 
        var min_y = newElement.blocks.Min(s => s.coordinates.y);
        var max_y = newElement.blocks.Max(s => s.coordinates.y);

        var size = max_y - min_y;

        int currentHeightPosition  = (_matrix.height - _minHeight) * _gameCamera.lastMaxCurrentHeight / _heightHandler.limitHeight + _minHeight; //(_matrix.height - _minHeight) * _heightHandler.currentHeight / _heightHandler.limitHeight + _minHeight;
        
        newElement.InitializationAfterGeneric(currentHeightPosition);
        newElement.myTransform.position = new Vector3(pos.x, pos.y + currentHeightPosition - size, pos.z);

        SetRandomPosition(newElement);
        //ConfuseElement(newElement);
     //   ConfuseElement(newElement);//, plane.gameObject);
        
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

    /*private Element GenerateConcretElement()
    {
        
    }*/
    
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
       // _answerElementParent.SetActive(true);
        Vector3Int stabiliation = new Vector3Int(1, 0, 1);
        
        foreach (var item in element.blocks)
        {
            var position = item.myTransform.position;
            Vector3Int v = stabiliation + new Vector3Int((int) position.x, (int) position.y, (int) position.z);
            _pool.CreateBlock(v, _answerElement, _BonusMaterial);
        }

        foreach (var block in _answerElement.blocks)
        {
            block.transform.localScale = Vector3.one * 0.9f;
        }
        
        var answerPosition = _answerElement.myTransform.position;
        answerPosition = new Vector3(answerPosition.x, 0.42f + _minPoint.y, answerPosition.z);
        _answerElement.myTransform.position = answerPosition; 
        //_answerElement.gameObject.SetActive(true);
    }

    public void DestroyOldDuplicate()
    {
        foreach (var block in _answerElement.blocks)
        {
            block.transform.localScale = Vector3.one * 0.97f;
            _pool.DeleteBlock(block);
        }
        _answerElement.RemoveBlocksInList(_answerElement.blocks.ToArray());
        //_answerElement.gameObject.SetActive(false);
    }

    public void ShowAnswerElement()
    {
        _answerElement.gameObject.SetActive(true);
    }
    
    #region RandomMove

    private void SetRandomPosition(Element element)
    {
        // first step
        int x_min, z_min, x_max, z_max;
        x_min = z_min = _matrix.wight;
        x_max = z_max = 0;

        foreach (var block in element.blocks)
        {
            x_max = Mathf.Max(block._coordinates.x, x_max);
            z_max = Mathf.Max(block._coordinates.z, z_max);
            
            x_min = Mathf.Min(block._coordinates.x, x_min);
            z_min = Mathf.Min(block._coordinates.z, z_min);
        }
        Vector3Int size = new Vector3Int(x_max - x_min,0, z_max - z_min);
        
        //second step
        int minCoordinate = 0.ToCoordinat();
        foreach (var block in element.blocks)
        {
            block.OffsetCoordinates(minCoordinate - x_min, 0,  minCoordinate - z_min);
        }
        MoveInfluence.MomentaryMove(element, new Vector3(minCoordinate - x_min, 0, minCoordinate - z_min));

        //third step
        int x_move, z_move;
        
        x_move = Random.Range(0, _matrix.wight - size.x);
        z_move = Random.Range(0,_matrix.wight - size.z);
        
        foreach (var block in element.blocks)
        {
            block.OffsetCoordinates(x_move, 0,  z_move);
        }
        MoveInfluence.MomentaryMove(element, new Vector3(x_move, 0, z_move));
    }
    
    #endregion
}