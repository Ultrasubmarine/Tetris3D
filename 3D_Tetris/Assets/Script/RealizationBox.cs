using Script.GameLogic.TetrisElement;
using Script.Influence;
using UnityEngine;
using UnityEngine.Serialization;

public class RealizationBox : Singleton<RealizationBox>
{
    [SerializeField] private TetrisFSM _FSM;
    [Space(5)] 
    [SerializeField] private PlaneMatrix _matrix;
    [SerializeField] private Generator _generator;
    [SerializeField] private GameLogicPool _gameLogicPool;
    [FormerlySerializedAs("_elementManager")]
    [Space(5)] 
    [SerializeField] private ElementDropper _elementDropper;
    [SerializeField] private ElementCleaner _elementCleaner;
    [Space(5)] 
    [SerializeField] private Score _score;
    [SerializeField] private InfluenceManager _influenceManager;

    [SerializeField] private GameObject _controller;
    [SerializeField] private HeightHandler _heightHandler;

    [SerializeField] private GameCamera _gameCamera;
    [SerializeField] private MoveTouchController _moveTouchController;
    [SerializeField] private MovePointsManager _movePointsManager;
    [SerializeField] private GameController _gameController;
    
    public TetrisFSM FSM => _FSM;
    public PlaneMatrix matrix => _matrix;
    public Generator generator => _generator;
    public GameLogicPool gameLogicPool => _gameLogicPool;
    public ElementDropper elementDropper => _elementDropper;
    public ElementCleaner elementCleaner => _elementCleaner;
    public Score score => _score;
    public InfluenceManager influenceManager => _influenceManager;
    public GameObject controller => _controller;
    public HeightHandler haightHandler => _heightHandler;
    public GameCamera gameCamera => _gameCamera;
    public MoveTouchController moveTouchController => _moveTouchController;
    public MovePointsManager movePointsManager => _movePointsManager;
    public GameController gameController => _gameController;
    
    private void Start()
    {
        ElementData.Loader = () => { return _generator.GenerationNewElement(_elementDropper.transform); };
    }
}