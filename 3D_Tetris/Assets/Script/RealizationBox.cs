using Script.GameLogic.TetrisElement;
using Script.ObjectEngine;
using UnityEngine;

public class RealizationBox : Singleton<RealizationBox>
{
    [SerializeField] private TetrisFSM _FSM;
    [Space(5)] 
    [SerializeField] private PlaneMatrix _matrix;
    [SerializeField] private Generator _generator;
    [SerializeField] private GameLogicPool _gameLogicPool;
    [Space(5)] 
    [SerializeField] private ElementManager _elementManager;
    [SerializeField] private ElementCleaner _elementCleaner;
    [Space(5)] 
    [SerializeField] private Score _score;
    [SerializeField] private InfluenceManager _influenceManager;

    public TetrisFSM FSM => _FSM;
    public PlaneMatrix matrix => _matrix;
    public Generator generator => _generator;
    public GameLogicPool gameLogicPool => _gameLogicPool;
    public ElementManager elementManager => _elementManager;
    public ElementCleaner elementCleaner => _elementCleaner;
    public Score score => _score;
    public InfluenceManager influenceManager => _influenceManager;
    
    private void Start()
    {
        ElementData.Loader = () => { return _generator.GenerationNewElement(_elementManager.transform); };
    }
}