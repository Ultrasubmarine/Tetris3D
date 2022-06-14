using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;
using Script.GameLogic;
using Script.GameLogic.Bomb;
using Script.GameLogic.StoneBlock;
using Script.Influence;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct IndexVector
{
    public Vector3Int newPoint;
    public Vector3Int parentPoint;

    public IndexVector(Vector3Int _newPoint, Vector3Int _parentPoint)
    {
        newPoint = _newPoint;
        parentPoint = _parentPoint;
    }
}

[Serializable]
public struct ProbabilitySettings
{
    public int layerDown;
    public int neighbours;
    public int startPoints;
    public ProbabilitySettings(int layerDown, int neighbours, int startPoints)
    {
        this.layerDown = layerDown;
        this.neighbours = neighbours;
        this.startPoints = startPoints;
    }
}


public enum ElementType
{
    none,
    element,
    bomb,
    bigBomb,
    evilBox,
}
public struct AbstractElementInfo
{
    public ElementType type;
    public List<Vector3Int> blocks;
    public Material material;
    public List<CoordinatXZ> matrixCoordinat;
    public bool stone;
}

public class Generator : MonoBehaviour
{
    private GameLogicPool _pool;

    public AbstractElementInfo nextElement => _nextElement;
    public Action<AbstractElementInfo> onNextElementGenerated;
    
    [SerializeField] private Material[] _MyMaterial;
    [SerializeField] private GameObject _answerElementParent;
    public Material freezeMaterial;

    [Space(10)] [Header("GD Balance height generate element")]
    [SerializeField] private int _minHeight;
    
    public bool _generateNeedElement = false;
    public bool exceptCurrentElementForNext = false;
    
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private GameCamera _gameCamera;
    private BombsManager _bombsManager;
    private EvilBoxManager _evilBoxManager;
    private StoneBlockManager _stoneBlockManager;
    
    private bool[,,] _castMatrix;
    private Vector3Int _minPoint;
    
    private AbstractElementInfo _nextElement;
    
    public int fixedHightPosition = 0;

    [SerializeField] public int stepOfHardElement = 2; // 1-min 3-max
    [SerializeField] public bool growBlocksAnywhere = false; // grow more hard element
    
    [SerializeField] public ProbabilitySettings _probabilitySettings = new ProbabilitySettings(20,5,10);

    private Element _lastGenerateElement;

    [SerializeField] private Transform _mini;

    [SerializeField] private LineBanManager _lineBanManager;
    
    private void Start()
    {
        _matrix = RealizationBox.Instance.matrix;
        _pool = RealizationBox.Instance.gameLogicPool;
        _heightHandler = RealizationBox.Instance.haightHandler;
        _gameCamera = RealizationBox.Instance.gameCamera;
        _bombsManager = RealizationBox.Instance.bombsManager;
        _evilBoxManager = RealizationBox.Instance.evilBoxManager;
        _stoneBlockManager = RealizationBox.Instance.stoneBlockManager;
        
        _castMatrix = new bool[3, 5, 3];
        
        _nextElement = new AbstractElementInfo();
        _nextElement.type = ElementType.none;
        _nextElement.blocks = new List<Vector3Int>();
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
      
        if (_nextElement.type == ElementType.none)
        { 
            GenerateNextElement(false);
        }
      
        var newElement =  GenerateElementByNext();
        GenerateNextElement();
        
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
        _lastGenerateElement = newElement;
        return newElement;
    }

    public void SetNextAsBigBomb()
    {
        _nextElement.type = ElementType.bigBomb;
        onNextElementGenerated?.Invoke(_nextElement);
    }
    public void GenerateNextElement(bool callback = true)
    {
        List<Vector3Int> exceptPositions = new List<Vector3Int>();
        if (exceptCurrentElementForNext && !Equals(nextElement.blocks,null) && nextElement.type == ElementType.element)
        {
            exceptPositions = _nextElement.blocks;
        }

        if (_bombsManager.CanMakeBomb())
        {
            _nextElement.type = ElementType.bomb;
            _evilBoxManager.IncrementStep();
        }
        else if (_evilBoxManager.CanMakeEvilBox())
        {
            _nextElement.type = ElementType.evilBox;
        }
        else
        {
            _castMatrix = CreateCastMatrix(_minPoint.y);
            
            if(exceptPositions.Count > 0)
                ExceptPositionsInCastMatrix(ref _castMatrix, exceptPositions);
            
            List<Vector3Int> blockPositions;

            do
            {
                blockPositions = GenerateBlocksCoordinates();

                if (!_lineBanManager.CanCreateElement(blockPositions))
                {
                    foreach (var b in blockPositions)
                    {
                        _castMatrix[b.x, b.y, b.z] = true; // clear
                    }

                    blockPositions.Clear();
                }
                else
                {
                    break;
                }
            
            } while (true);

            _nextElement.type = ElementType.element;
            
            _nextElement.blocks.Clear();
            _nextElement.blocks = blockPositions;
            _nextElement.material = _MyMaterial[ Random.Range(0, _MyMaterial.Length - 1)];

            if (_stoneBlockManager.CanTransformToStone())
            {
                _nextElement.stone = true;
                _nextElement.material = _stoneBlockManager.blockMaterial;
            }
            else
                _nextElement.stone = false;
        }

        if (callback)
        {
            onNextElementGenerated?.Invoke(_nextElement);
        }
    }
    
    private bool[,,] CreateCastMatrix(int min)
    {
        int y_size = _heightHandler.limitHeight;
        var castMatrix = new bool[3, y_size, 3];
        int barrier;

        for (var x = 0; x < 3; x++)
        for (var z = 0; z < 3; z++)
        {
            barrier = _matrix.MinHeightInCoordinates(x, z);
            for (var y = min + y_size - 1; y >= min; y--) castMatrix[x, y - min, z] = y >= barrier;
        }

        return castMatrix;
    }

    private void ExceptPositionsInCastMatrix(ref bool[,,] castMatrix, List<Vector3Int> positions)
    {
        foreach (var p in positions)
        {
            for (int i = 0; i < _heightHandler.limitHeight; i++)
            {
                castMatrix[p.x, i, p.z] = false;
            }
        }
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
                int amount;
                if(HasNeibhords(new Vector3Int(x,y,z), 1, out amount))
                    place.Add(new Vector3Int(x, y, z));
                break;
            }
        }
        return place;
    }

    private bool HasNeibhords(Vector3Int point, int delay, out int amountNeighbours)
    {
        int amountOfNeighbords = 0;
        int weight = 0;

        int posNeibhord;
        
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(1, 0, 0), point.y, point.y + delay);
        if (posNeibhord  != 404 && posNeibhord != -1)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(-1, 0, 0), point.y, point.y + delay);
        if (posNeibhord  != 404 && posNeibhord != -1)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(0, 0, 1), point.y, point.y + delay);
        if (posNeibhord  != 404 && posNeibhord != -1)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }
        posNeibhord = IsMinimalYBetween(point + new Vector3Int(0, 0, -1), point.y, point.y + delay);
        if (posNeibhord  != 404 && posNeibhord != -1)
        {
            weight += posNeibhord;
            amountOfNeighbords++;
        }

        amountNeighbours = amountOfNeighbords;
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
    
    private Element GenerateElementByNext()
    {
        if (_nextElement.type == ElementType.bomb)
            return _bombsManager.MakeBomb();
        else if (_nextElement.type == ElementType.bigBomb)
            return _bombsManager.MakeBomb(true);
        else if (_nextElement.type == ElementType.evilBox)
            return _evilBoxManager.MakeEvilBox();
        
        var createElement = _pool.CreateEmptyElement();

        foreach (var p in _nextElement.blocks)
        {
            _pool.CreateBlock(p, createElement, _nextElement.material);
        }

        if (_nextElement.stone)
        {
            _stoneBlockManager.TransformToStone(createElement);
        }
        return createElement;
    }

    private List<Vector3Int> GenerateBlocksCoordinates()
    {
        List<Vector3Int> position = new List<Vector3Int>();
        
        var firstPoint = GetStartEmptyPosition();
        var lastPoint = firstPoint;
        _castMatrix[firstPoint.x, firstPoint.y, firstPoint.z] = false;

        Vector3Int deltaY = new Vector3Int(0, firstPoint.y, 0);
        position.Add( lastPoint - deltaY);
            
        List<IndexVector> freePlaces = new List<IndexVector>();
        List<Vector3Int> generatePoints = new List<Vector3Int>();
            
        generatePoints.Add(lastPoint);
        var elementComplexity = Vector3Int.zero;
        for (var i = 0; i < 3; i++)
        {
            if (growBlocksAnywhere)
            {
                freePlaces.Clear();
                foreach (var block in generatePoints)
                {
                    freePlaces.AddRange(FoundFreePlacesAround(firstPoint, block, elementComplexity));
                }
            }
            else
                freePlaces = FoundFreePlacesAround(firstPoint, lastPoint, elementComplexity);
            if (freePlaces.Count == 0)
                break;
                
            var generatePoint = freePlaces[Random.Range(0, freePlaces.Count)];
            var different = generatePoint.parentPoint - generatePoint.newPoint;
            lastPoint = generatePoint.newPoint;
             
            position.Add( lastPoint - deltaY);
            _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;
            generatePoints.Add(lastPoint);
                
            if (different.x != 0)
                elementComplexity.x = 1;
            else if (different.y != 0)
                elementComplexity.y = 1;
            else if (different.z != 0)
                elementComplexity.z = 1;
        }
        return position;
    }

    private Vector3Int GetStartEmptyPosition() // for grow element 
    {
        var emptyPlaces = CalculateEmptyPlaceInCastMatrix();

        emptyPlaces.Sort((a, b) => { return a.y - b.y;});

        if (_generateNeedElement)
            return emptyPlaces[0]; // min 

        int max_y = emptyPlaces.Max((i => i.y));

        List<float> percents = new List<float>();

        int sum = 0;
        float onePr;

       
        foreach (var place in emptyPlaces) // set points for all places 
        {
            int amount;
            HasNeibhords(place, 1, out amount);
            
            int points = (max_y - place.y) * _probabilitySettings.layerDown + amount * _probabilitySettings.neighbours 
                                                                            + _probabilitySettings.startPoints; // rule for points 
            percents.Add(points);
            sum += points;
        }
        onePr = sum * 0.01f;
        
        float p = Random.Range(0, 100);
        float currentLine = 0;
        
        string debugstr = "";
        // convert points to percent
        for (int i=0; i< percents.Count; i++)
        {
            percents[i] /= onePr;
            
            debugstr += percents[i].ToString();
            if (i % 3 == 0)
                debugstr += "\n";
            else 
                debugstr += "   ";
        }
        
        //Debug.Log(debugstr);
        for (int i=0; i< percents.Count; i++)
        {
            if (percents[i] + currentLine > p)
            {
             //   Debug.Log("choose" + i);
                return emptyPlaces[i];
            }

            currentLine += percents[i];
        }
        
        return emptyPlaces[emptyPlaces.Count - 1];
    }
    
    private List<IndexVector> FoundFreePlacesAround(Vector3Int firstPoint, Vector3Int point, Vector3Int elementComplexity, bool ignoreCastBlocks = false)
    {
        var listPov = new List<IndexVector>();
        
        if (CheckEmptyPlace(point + new Vector3Int(1, 0, 0),ignoreCastBlocks) 
            && (1 + elementComplexity.y + elementComplexity.z <= stepOfHardElement))
            listPov.Add(new IndexVector (point + new Vector3Int(1, 0, 0),point));

        if (CheckEmptyPlace(point + new Vector3Int(-1, 0, 0),ignoreCastBlocks)
            && (1 + elementComplexity.y + elementComplexity.z <= stepOfHardElement))
            listPov.Add(new IndexVector (point + new Vector3Int(-1, 0, 0),point));
        
        if (CheckEmptyPlace(point + new Vector3Int(0, 0, 1),ignoreCastBlocks)
            && (elementComplexity.x + elementComplexity.y + 1 <= stepOfHardElement))
            listPov.Add(new IndexVector (point + new Vector3Int(0, 0, 1),point));
        
        if (CheckEmptyPlace(point + new Vector3Int(0, 0, -1),ignoreCastBlocks)
            && (elementComplexity.x + elementComplexity.y + 1 <= stepOfHardElement))
            listPov.Add(new IndexVector (point + new Vector3Int(0, 0, -1),point));

        if (point.y - firstPoint.y < 2)//(point.y < 7 - 1)//2)
            if (CheckEmptyPlace(point + new Vector3Int(0, 1, 0),ignoreCastBlocks)
                && (elementComplexity.x + 1 + elementComplexity.z <= stepOfHardElement))
                listPov.Add(new IndexVector (point + new Vector3Int(0, 1, 0),point));

        return listPov;
    }

    private bool CheckEmptyPlace(Vector3Int indices, bool ignoreCastBlocks = false)
    {
        if (indices.OutOfIndexLimit())
            return false;

        if (ignoreCastBlocks)
            return true;
        
        if (indices.y != 0 && _castMatrix[indices.x, indices.y - 1, indices.z])
            return false;

        return _castMatrix[indices.x, indices.y, indices.z];
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
        z_move = Random.Range(0, _matrix.wight - size.z);

        foreach (var block in element.blocks)
        {
            block.OffsetCoordinates(x_move, 0,  z_move);
        }
        MoveInfluence.MomentaryMove(element, new Vector3(x_move, 0, z_move));
    }
    
    public void SetRandomPositionForEvilBlox(Element element, List<CoordinatXZ> exceptList = null)
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
        CoordinatXZ newCoordinats; // for evilbox only
        
        do
        {
            x_move = Random.Range(0, _matrix.wight - size.x + 1);
            z_move = Random.Range(0, _matrix.wight - size.z + 1);

            newCoordinats = new CoordinatXZ(element.blocks[0].coordinates.x + x_move,element.blocks[0].coordinates.z + z_move);

        } while (exceptList != null && exceptList.Contains(newCoordinats)); // veeeery bad code

        foreach (var block in element.blocks)
        {
            block.OffsetCoordinates(x_move, 0,  z_move);
        }
        MoveInfluence.MomentaryMove(element, new Vector3(x_move, 0, z_move));
    }
    #endregion

    public void Clear()
    {
        if (!Equals(_lastGenerateElement, null) && _lastGenerateElement.gameObject.activeInHierarchy)
        {
            foreach (var item in _lastGenerateElement.blocks.ToArray()) if(!Equals(item, null))_pool.DeleteBlock(item);
            _lastGenerateElement.RemoveBlocksInList(_lastGenerateElement.blocks.ToArray());
            _pool.DeleteElement(_lastGenerateElement);
        }
        
        _nextElement.blocks.Clear();
        _nextElement.type = ElementType.none;
        onNextElementGenerated.Invoke(_nextElement);
    }
}