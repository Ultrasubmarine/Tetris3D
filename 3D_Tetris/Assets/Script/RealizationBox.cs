using System;
using System.Collections.Generic;
using Script;
using Script.Controller;
using Script.Controller.TouchController;
using Script.GameLogic;
using Script.GameLogic.Bomb;
using Script.GameLogic.Stars;
using Script.GameLogic.StoneBlock;
using Script.GameLogic.TetrisElement;
using Script.Influence;
using Script.Offers;
using Script.Projections;
using Script.Speed;
using UnityEngine;
using UnityEngine.Serialization;

public class RealizationBox : Singleton<RealizationBox>
{
    [SerializeField] private TetrisFSM _FSM;
    [SerializeField] private GameManager _gameManager;
    [Space(5)] [SerializeField] private PlaneMatrix _matrix;
    [SerializeField] private Generator _generator;
    [SerializeField] private GameLogicPool _gameLogicPool;
    [Space(5)] [SerializeField] private ElementDropper _elementDropper;
    [SerializeField] private ElementCleaner _elementCleaner;
    [Space(5)] [SerializeField] private Score _score;
    [SerializeField] private InfluenceManager _influenceManager;

    [SerializeField] private GameObject _controller;
    [SerializeField] private HeightHandler _heightHandler;

    [SerializeField] private GameCamera _gameCamera;
    [SerializeField] private MoveTouchController _moveTouchController;
    [SerializeField] private GameController _gameController;

    [SerializeField] private SlowManager _slowManager;
    [SerializeField] private Transform _place;
    [SerializeField] private Speed _speed;

    [SerializeField] private SpeedChanger _speedChanger;
    [SerializeField] private GeneratorChanger _generatorChanger;

    [Header("UI GameControllers")] [SerializeField]
    private MovementJoystick _joystick;

    [SerializeField] private TapsEvents _tapsEvents;
    [SerializeField] private IslandTurn _islandTurn;

    [Header("Projections")] [SerializeField]
    private ProjectionLineManager _projectionLineManager;

    [SerializeField] private Projection _projection;
    [SerializeField] private Ceiling _ceiling;

    [SerializeField] private StarsManager _starsManager;
    [SerializeField] private StarUIAnimation _starUIAnimation;

    [Header("Tutorials")]
    [SerializeField] private GameObject _joystickTutor;
    [SerializeField] private GameObject _starsTutor;
    [SerializeField] private StarUI _starUI;
    [SerializeField] private LvlElementsSetter _lvlElementsSetter;
    [SerializeField] private DestroyedLayerParticles _destroyedLayerParticles;

    [SerializeField] private BombsManager _bombsManager;
    [SerializeField] private PauseUI _pauseUI;
    [SerializeField] private BigBombGamePlayOffer _bigBombGamePlayOffer;
    [SerializeField] private ChangeNewElementToBomb _changeNewElementToBomb;

    [SerializeField] private NextElementUI _nextElementUI;
    [SerializeField] private EvilBoxManager _evilBoxManager;
    [SerializeField] private StoneBlockManager _stoneBlockManager;
    [SerializeField] private FastElementDrop _fastElementDrop;
    
    public TetrisFSM FSM => _FSM;
    public GameManager gameManager => _gameManager;
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
    public GameController gameController => _gameController;
    public SlowManager slowManager => _slowManager;
    public Transform place => _place;
    public Speed speed => _speed;
    public SpeedChanger speedChanger => _speedChanger;
    public GeneratorChanger generatorChanger => _generatorChanger;
    public ProjectionLineManager projectionLineManager => _projectionLineManager;
    public Projection projection => _projection;
    public MovementJoystick joystick => _joystick;
    public TapsEvents tapsEvents => _tapsEvents;
    public IslandTurn islandTurn => _islandTurn;
    public Ceiling ceiling => _ceiling;
    public StarsManager starsManager => _starsManager;
    public StarUIAnimation starUIAnimation => _starUIAnimation;
    public StarUI starUI => _starUI;
    public LvlElementsSetter lvlElementsSetter => _lvlElementsSetter;
    public DestroyedLayerParticles destroyedLayerParticles => _destroyedLayerParticles;
    public BombsManager bombsManager => _bombsManager;
    public BigBombGamePlayOffer bigBombGamePlayOffer => _bigBombGamePlayOffer;
    public ChangeNewElementToBomb changeNewElementToBomb => _changeNewElementToBomb;
    
    public NextElementUI nextElementUI => _nextElementUI;
    public PauseUI pauseUI => _pauseUI;
    public EvilBoxManager evilBoxManager => _evilBoxManager;
    public StoneBlockManager stoneBlockManager => _stoneBlockManager;
    public  FastElementDrop fastElementDrop => _fastElementDrop;
    
    private Dictionary<TutorType, GameObject> _tutors;
    
    public GameObject GetTutor(TutorType t)
    {
        return _tutors[t];
    }

    protected override void Init()
    {
        _tutors = new Dictionary<TutorType, GameObject>();
        _tutors[TutorType.Joystick] = _joystickTutor;
        _tutors[TutorType.Stars] = _starsTutor;
    }

    private void Start()
    {
        ElementData.Instance.loader = () => { return _generator.GenerationNewElement(_elementDropper.transform); };
    }
}