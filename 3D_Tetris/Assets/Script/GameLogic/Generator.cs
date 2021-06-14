using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;
using Script.Influence;
using Random = UnityEngine.Random;

public struct PlacePoint
{
    public Vector3Int position;
    public float p;
}

public class Generator : MonoBehaviour
{
    private GameLogicPool _pool;

    [SerializeField] private Material[] _MyMaterial;
    [SerializeField] private GameObject _answerElementParent;
    public Material freezeMaterial;

    [Space(10)] [Header("GD Balance height generate element")]
    [SerializeField] private int _minHeight;

    [Tooltip(" подсказка места расположения падающего элемента")] [SerializeField]
    private Material _BonusMaterial;

    public float _pGenerateNeedElement = 0.5f;
    
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private GameCamera _gameCamera;
    
    private bool[,,] _castMatrix;
    private Vector3Int _minPoint;

    public Element _answerElement;

    public int fixedHightPosition = 0;

    [SerializeField] private int stepOfHardElement = 2; // 1-min 3-max
    
    private void Start()
    {
        _matrix = RealizationBox.Instance.matrix;
        _pool = RealizationBox.Instance.gameLogicPool;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _gameCamera = RealizationBox.Instance.gameCamera;

        _castMatrix = new bool[3, 5, 3];
        
        _answerElement= _pool.CreateEmptyElement();
        _answerElement.myTransform.parent = _answerElementParent.transform;
        _answerElement.myTransform.position = new Vector3(0,0.42f, 0);
        _answerElement.gameObject.SetActive(false);
    }

    public void GeneratePickableBlock()
    {
       var pBlock = _pool.CreatePickableBlock(new Vector3Int(0, _matrix.limitHeight -5, 0));
       _matrix.BindBlock(pBlock);
       RealizationBox.Instance.projectionLineManager.AddPickableProjection(pBlock);
    }
    public Element GenerationNewElement(Transform elementParent)
    {
        _minPoint = _matrix.FindLowerAccessiblePlace();
        _castMatrix = CreateCastMatrix(_minPoint.y);

      //  GeneratePickableBlock();
      
        var newElement = GenerateElement();
        
        CreateDuplicate(newElement);

        var pos = elementParent.position;

        // выравниваем элемент относительно координат y 
        var min_y = newElement.blocks.Min(s => s.coordinates.y);
        var max_y = newElement.blocks.Max(s => s.coordinates.y);

        var size = max_y - min_y;

        int currentHeightPosition  = (_matrix.height - _minHeight) * _gameCamera.lastMaxCurrentHeight / _heightHandler.limitHeight + _minHeight; //(_matrix.height - _minHeight) * _heightHandler.currentHeight / _heightHandler.limitHeight + _minHeight;

        int currentYpos = fixedHightPosition == 0 ? currentHeightPosition : fixedHightPosition;
        newElement.InitializationAfterGeneric(currentYpos);
        newElement.myTransform.position = new Vector3(pos.x, pos.y + currentYpos - size, pos.z);

        SetRandomPosition(newElement);
        //ConfuseElement(newElement);
     //   ConfuseElement(newElement);//, plane.gameObject);

        fixedHightPosition = 0;
        return newElement;
    }

    private bool[,,] CreateCastMatrix(int min)
    {
        var castMatrix = new bool[3, 7, 3];
        int barrier;

        for (var x = 0; x < 3; x++)
        for (var z = 0; z < 3; z++)
        {
            barrier = _matrix.MinHeightInCoordinates(x, z);
            for (var y = min + 7 - 1; y >= min; y--) castMatrix[x, y - min, z] = y >= barrier;
        }

        return castMatrix;
    }

    private List<Vector3Int> CalculateEmptyPlaceInCastMatrix()
    {
        List<Vector3Int> place = new List<Vector3Int>();
        
        for (var x = 0; x < _castMatrix.GetLength(0); x++)
        for (var z = 0; z < _castMatrix.GetLength(2); z++)
        for (var y = 0; y < _castMatrix.GetLength(1)-3; y++)
        {
            if (_castMatrix[x, y, z]) // empty
            {
                if(HasNeibhords(new Vector3Int(x,y,z), 1))
                    place.Add(new Vector3Int(x, y, z));
                break;
            }
        }
        return place;
    }

    private bool HasNeibhords(Vector3Int point, int delay)
    {
        int amountOfNeighbords = 0;
        int weight = 0;

        int posNeibhord;
        
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(1, 0, 0), point.y, point.y + delay);
        if (posNeibhord  != 404)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(-1, 0, 0), point.y, point.y + delay);
        if (posNeibhord  != 404)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(0, 0, 1), point.y, point.y + delay);
        if (posNeibhord  != 404)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(0, 0, -1), point.y, point.y + delay);
        if (posNeibhord  != 404)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        
        if(amountOfNeighbords + weight == 0) // the tallest
            return false;
        return true;
    }

    // 0 - if include [min, max)
    // 1 - if > max
    // -1 - if < min
    private int IsMinimalYBetween(Vector3Int point, int minY, int maxY)
    {
        if (point.OutOfIndexLimit())
            return 404; // not found 

        for (var y = 0; y < _castMatrix.GetLength(1) && y < maxY; y++)
        {
            if (_castMatrix[point.x, y, point.z]) // empty
            {
                if( y >= minY)
                    return 0;
                return -1;
            }
        }
        return 1;
    }
    
    private Element GenerateElement()
    {
        var indexMat = Random.Range(0, _MyMaterial.Length - 1);

        var createElement = _pool.CreateEmptyElement();

        var emptyPlaces = CalculateEmptyPlaceInCastMatrix();
        Debug.Log($"cont place: {emptyPlaces.Count}");
        int randomIndexPlace = Random.Range(0, emptyPlaces.Count());

        var firstPoint = emptyPlaces[randomIndexPlace];
        var lastPoint = emptyPlaces[randomIndexPlace];
        _castMatrix[emptyPlaces[randomIndexPlace].x, emptyPlaces[randomIndexPlace].y, emptyPlaces[randomIndexPlace].z] = false;

        Vector3Int deltaY = new Vector3Int(0, firstPoint.y, 0);
        _pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);

        List<Vector3Int> freePlaces;
        
        Vector3Int elementComplexity = Vector3Int.zero;
        for (var i = 0; i < 3; i++)
        {
            freePlaces = FoundFreePlacesAround(firstPoint, lastPoint, elementComplexity);
            if (freePlaces.Count == 0)
                break;
            
            var generatePoint = freePlaces[Random.Range(0, freePlaces.Count)];
            var different = lastPoint - generatePoint;
            lastPoint = generatePoint;
            
            _pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;

            if (different.x != 0)
                elementComplexity.x = 1;
            else if (different.y != 0)
                elementComplexity.y = 1;
            else if (different.z != 0)
                elementComplexity.z = 1;
        }
        return createElement;
    }

    private List<Vector3Int> FoundFreePlacesAround(Vector3Int firstPoint, Vector3Int point, Vector3Int elementComplexity)
    {
        var listPov = new List<Vector3Int>();
        var complexity = elementComplexity.x + elementComplexity.y + elementComplexity.z;
        
        if (CheckEmptyPlace(point + new Vector3Int(1, 0, 0)) 
            && (1 + elementComplexity.y + elementComplexity.z <= stepOfHardElement))
            listPov.Add(point + new Vector3Int(1, 0, 0));

        if (CheckEmptyPlace(point + new Vector3Int(-1, 0, 0))
            && (1 + elementComplexity.y + elementComplexity.z <= stepOfHardElement))
            listPov.Add(point + new Vector3Int(-1, 0, 0));
        
        if (CheckEmptyPlace(point + new Vector3Int(0, 0, 1))
            && (elementComplexity.x + elementComplexity.y + 1 <= stepOfHardElement))
            listPov.Add(point + new Vector3Int(0, 0, 1));
        
        if (CheckEmptyPlace(point + new Vector3Int(0, 0, -1))
            && (elementComplexity.x + elementComplexity.y + 1 <= stepOfHardElement))
            listPov.Add(point + new Vector3Int(0, 0, -1));

        if (point.y - firstPoint.y < 2)//(point.y < 7 - 1)//2)
            if (CheckEmptyPlace(point + new Vector3Int(0, 1, 0))
                && (elementComplexity.x + 1 + elementComplexity.z <= stepOfHardElement))
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

    public void SetRandomPosition(Element element)
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