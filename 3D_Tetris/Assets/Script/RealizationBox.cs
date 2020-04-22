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
    [FormerlySerializedAs("_movePointsManager")] [SerializeField] private PointJoystick _pointJoystick;
    [SerializeField] private GameController _gameController;

    [SerializeField] private SlowManager _slowManager;
    [SerializeField] private Transform _place;
    
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
    public PointJoystick pointJoystick => _pointJoystick;
    public GameController gameController => _gameController;
    public SlowManager slowManager => _slowManager;
    public Transform place => _place;
    
    private void Start()
    {
        ElementData.loader = () => { return _generator.GenerationNewElement(_elementDropper.transform); };
    }
}