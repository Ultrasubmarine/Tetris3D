using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IntegerExtension;
using Script.GameLogic;
using Script.GameLogic.Bomb;
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

[Serializable]
public struct BanLineElement
{
    public bool isBan;
    
    public int amountForBan; // if line generated so many times 
    public int currentAmountForBan;

    //step after ban
    public int freezeSteps;
    public int currentFreeze;
    
    public BanLineElement(bool isBan, int amountForBan, int freezeSteps)
    {
        this.isBan = isBan;
        
        this.amountForBan = amountForBan;
        this.freezeSteps = freezeSteps;
        
        this.currentFreeze = 0;
        currentAmountForBan = 0;
    }
    
    public void IncrementFreezeStep()
    {
        if (currentFreeze + 1 == freezeSteps)
        {
            isBan = false;
            currentFreeze = 0;
        }
        else
        {
            currentFreeze++; 
        }
    }
    public void IncrementLine()
    {
        if (currentAmountForBan + 1 == amountForBan)
        {
            isBan = true;
            currentAmountForBan = 0;
        }
        else
        {
            currentAmountForBan++;
        }
    }
    public void ClearLine()
    {
        this.currentAmountForBan = 0;
    }

    public void NotLine()
    {
        if (isBan)
            IncrementFreezeStep();
        else
            ClearLine();
    }
}

public struct AbstractElementInfo
{
    public List<Vector3Int> blocks;
    public Material material;
    public List<CoordinatXZ> matrixCoordinat;
}

public class Generator : MonoBehaviour
{
    private GameLogicPool _pool;

    public AbstractElementInfo abstractElementInfo => _abstractElementInfo;
    public Action<AbstractElementInfo> onNextElementGenerated;
    
    [SerializeField] private Material[] _MyMaterial;
    [SerializeField] private GameObject _answerElementParent;
    public Material freezeMaterial;

    [Space(10)] [Header("GD Balance height generate element")]
    [SerializeField] private int _minHeight;

    [Tooltip(" подсказка места расположения падающего элемента")] [SerializeField]
    private Material _BonusMaterial;

    public bool _generateNeedElement = false;
    
    private PlaneMatrix _matrix;
    private HeightHandler _heightHandler;
    private GameCamera _gameCamera;
    private BombsManager _bombsManager;
    
    private bool[,,] _castMatrix;
    private Vector3Int _minPoint;

    public Element _answerElement;

    private AbstractElementInfo _abstractElementInfo;
    
    public int fixedHightPosition = 0;

    [SerializeField] public int stepOfHardElement = 2; // 1-min 3-max
    [SerializeField] public bool growBlocksAnywhere = false; // grow more hard element

    [FormerlySerializedAs("_blockLineElement")] [SerializeField] private BanLineElement banLineElement; // for chiters

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
            
        _castMatrix = new bool[3, 5, 3];
        
        _answerElement= _pool.CreateEmptyElement();
        //_answerElement.myTransform.parent = _answerElementParent.transform;
       // _answerElement.myTransform.position = new Vector3(0,0.42f, 0);
        _answerElement.transform.parent = _mini;
        _answerElement.transform.localPosition = Vector3.zero;
        _answerElement.transform.localRotation = Quaternion.identity;
        _answerElement.transform.localScale = Vector3.one * 70;

        RealizationBox.Instance.islandTurn.extraTurn.Add(_answerElement.myTransform);
        //   _answerElement.transform.parent = this.transform;

        //  _answerElement.gameObject.SetActive(false);
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

        var e = _bombsManager.MakeBomb();
        var newElement = e == null? GenerateElement(): e;
        
        CreateDuplicate(newElement,newElement.blocks[0].GetComponent<MeshRenderer>().material);

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
    
    private Element GenerateElement()
    {
        _castMatrix = CreateCastMatrix(_minPoint.y);
        List<Vector3Int> blockPositions;

        do
        {
            // // Debug.ClearDeveloperConsole();
            //  //Debug.Log("not change");
            //  if (isLineElement(createElement) && banLineElement.isBan)
            //  {
            //    //  Debug.ClearDeveloperConsole();
            //    //  Debug.Log("change pos");
            //      foreach (var b in createElement.blocks)
            //      {
            //          _castMatrix[b._coordinates.x.ToIndex(), b._coordinates.y, b._coordinates.z.ToIndex()] = true; // clear
            //          _pool.DeleteBlock(b);
            //      }
            //      createElement.blocks.Clear();
            //  }

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
            // var firstPoint = GetStartEmptyPosition();
            // var lastPoint = firstPoint;
            // _castMatrix[firstPoint.x, firstPoint.y, firstPoint.z] = false;
            //
            // Vector3Int deltaY = new Vector3Int(0, firstPoint.y, 0);
            //_pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
            // List<IndexVector> freePlaces = new List<IndexVector>();
            // List<Vector3Int> generatePoints = new List<Vector3Int>();
            //
            // generatePoints.Add(lastPoint);
            // Vector3Int elementComplexity = Vector3Int.zero;
            // for (var i = 0; i < 3; i++)
            // {
            //     if (growBlocksAnywhere)
            //     {
            //         freePlaces.Clear();
            //         foreach (var block in generatePoints)
            //         {
            //             freePlaces.AddRange(FoundFreePlacesAround(firstPoint, block, elementComplexity));
            //         }
            //     }
            //     else
            //         freePlaces = FoundFreePlacesAround(firstPoint, lastPoint, elementComplexity);
            //     if (freePlaces.Count == 0)
            //         break;
            //     
            //     var generatePoint = freePlaces[Random.Range(0, freePlaces.Count)];
            //     var different = generatePoint.parentPoint - generatePoint.newPoint;
            //     lastPoint = generatePoint.newPoint;
            //     
            //     _pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
            //     _castMatrix[lastPoint.x, lastPoint.y, lastPoint.z] = false;
            //     generatePoints.Add(lastPoint);
            //     
            //     if (different.x != 0)
            //         elementComplexity.x = 1;
            //     else if (different.y != 0)
            //         elementComplexity.y = 1;
            //     else if (different.z != 0)
            //         elementComplexity.z = 1;
            // }

            // if (isLineElement(createElement))
            // {
            //     if (banLineElement.isBan)
            //         //change element.Add random block
            //     {
            //         // elementComplexity = Vector3Int.zero;
            //         //
            //         // freePlaces.Clear();
            //         // freePlaces = FoundFreePlacesAround(lastPoint, lastPoint,elementComplexity, true);
            //         //
            //         // var generatePoint = freePlaces[Random.Range(0, freePlaces.Count)];
            //         // lastPoint = generatePoint.newPoint;
            //         // _pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
            //         // Debug.Log("cheat BLOCK CrEATED");
            //     }
            //     else
            //     {
            //         banLineElement.IncrementLine();
            //     }
            // }
            // else
            // {
            //     banLineElement.NotLine();
            // }

        } while (true); //(isLineElement(createElement) && banLineElement.isBan); // "generateCount < 15" - if creating another element is impossible
         
        var indexMat = Random.Range(0, _MyMaterial.Length - 1);
        var createElement = _pool.CreateEmptyElement();

        foreach (var p in blockPositions)
        {
            _pool.CreateBlock(p, createElement, _MyMaterial[indexMat]);
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
      //  _pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
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
            //_pool.CreateBlock(lastPoint - deltaY, createElement, _MyMaterial[indexMat]);
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
    private bool isLineElement(Element element)
    {
        if (element.blocks.Count != 3)
            return false;
      
        for (int i = 0; i < element.blocks.Count-1; i++)
        {
            if (element.blocks[i].xz != element.blocks[i + 1].xz)
                return false;
        }
        return true;
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

    private void CreateDuplicate( Element element, Material material)
    {
   //     _answerElement.transform.parent = this.transform;
        
       // _answerElementParent.SetActive(true);
        Vector3Int stabiliation = new Vector3Int(1, 0, 1);

        float xMax, zMax , yMax, xMin, zMin, yMin;
        xMax = zMax = yMax = int.MinValue;
        xMin = zMin = yMin = int.MaxValue;
        
        for(int i = 0; i<element.blocks.Count; i++)
        {
            var position = element.blocks[i]._coordinates;
            Vector3Int v = new Vector3Int((int) position.x, (int) position.y, (int) position.z);
            _pool.CreateBlock(v, _answerElement,material);

            Vector3 ansPos = _answerElement.blocks[i].myTransform.localPosition;
            xMax = xMax < ansPos.x? ansPos.x : xMax;
            zMax = zMax < ansPos.z? ansPos.z : zMax;
            yMax = yMax < ansPos.y? ansPos.y : yMax;
            
            xMin = xMin > ansPos.x? ansPos.x : xMin;
            zMin = zMin > ansPos.z? ansPos.z : zMin;
            yMin = yMin > ansPos.y? ansPos.y : yMin;
        }

        float xCenter, zCenter, yCenter;
        xCenter = (xMax + xMin) / 2f;
        zCenter = (zMax + zMin) / 2f;
        yCenter = (yMax + yMin) / 2f;
        
        foreach (var block in _answerElement.blocks)
        {
            Vector3 np = block.myTransform.localPosition - new Vector3(xCenter, yCenter, zCenter);
            block.myTransform.localPosition = np;
            block.myTransform.localScale = Vector3.one * 0.97f;
        }

        // var answerPosition = _answerElement.myTransform.position;
        // answerPosition = new Vector3(answerPosition.x, 0.42f + _minPoint.y, answerPosition.z);
        //  _answerElement.myTransform.position = answerPosition; 

       
      //  _answerElement.transform.localPosition = Vector3.zero;
        Debug.Log("local position: " +  _answerElement.transform.localPosition);
        _answerElement.gameObject.SetActive(true);
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

    public void Clear()
    {
        if (!Equals(_lastGenerateElement, null) && _lastGenerateElement.gameObject.activeInHierarchy)
        {
            foreach (var item in _lastGenerateElement.blocks.ToArray()) if(!Equals(item, null))_pool.DeleteBlock(item);
            _lastGenerateElement.RemoveBlocksInList(_lastGenerateElement.blocks.ToArray());
            _pool.DeleteElement(_lastGenerateElement);
        }
    }
}