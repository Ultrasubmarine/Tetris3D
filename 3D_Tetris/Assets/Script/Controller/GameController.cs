using System;
using DG.Tweening;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using Script.Influence;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public event Action<bool,move> onMoveApply;
        
    [SerializeField] private float _timeTurn = 0.3f;
    [SerializeField] private Button _turnButton;

    [SerializeField] private AudioClip _successMove;
    [SerializeField] private AudioClip _defeatMove;
    
    private move[] left_up = {move.zm, move.x, move.z, move.xm};
    private move[] left_down = {move.x, move.z, move.xm, move.zm};
    private move[] right_down = {move.z, move.xm, move.zm, move.x};
    private move[] right_up = {move.xm, move.zm, move.x, move.z};
    private int _indexTable = 0;

    private int _rotate = 0;
    private Transform _place;
    private bool _isTurn = false;
    
    public static bool MoveTutorial { get; set; }
    public static bool TurnTutorial { get; set; }

    private InfluenceManager _influence;
    private TetrisFSM _fsm;
    
    private AudioSource _audioSource;
    private void Start()
    {
        _influence = RealizationBox.Instance.influenceManager;
        _place = RealizationBox.Instance.place;
        _fsm = RealizationBox.Instance.FSM;

        _audioSource = GetComponent<AudioSource>();
    }
    

    public void Move(move touch)
    {
        if (Equals(ElementData.newElement))
            return;

        switch (touch)
        {
            case move.zm:
            {
                InfluenceData.direction = left_up[_indexTable];
                break;
            }
            case move.x:
            {
                InfluenceData.direction = left_down[_indexTable];
                break;
            }
            case move.z:
            {
                InfluenceData.direction = right_down[_indexTable];
                break;
            }
            case move.xm:
            {
                InfluenceData.direction = right_up[_indexTable];
                break;
            }
        }
        
        if(_fsm.GetCurrentState() == TetrisState.WaitInfluence){}
            _fsm.SetNewState(TetrisState.Move);
    }
 
    public void OnMoveActionApply(bool isSuccess, move direction)
    {
        onMoveApply.Invoke(isSuccess, direction);
        if (isSuccess)
            _audioSource.clip = _successMove;
        else
            _audioSource.clip = _defeatMove;
        _audioSource.Play();
    }
    
    public void Turn()
    {
        if (_isTurn)
            return;
        
        _isTurn = true;
        _influence.enabled = false;

        _rotate = (_rotate + 90) % 360;
        _indexTable = (_indexTable + 1) % 4;
        
        _place.DORotate(new Vector3(0, _rotate, 0), _timeTurn).OnComplete( () =>
        {
            _influence.enabled = true;
            _isTurn = false;
        });
    }

    public void CorrectTurn(int newAngle)
    {
        if (newAngle < 0)
            newAngle += 360;
        _indexTable = (newAngle) % 360  / 90 ;
    }
    
    private void OnFsmStateChange(TetrisState newState)
    {
        _fsm.AddListener(TetrisState.Drop, () => _turnButton.interactable = true);
        _fsm.AddListener(TetrisState.MergeElement, () => _turnButton.interactable = false);
        _fsm.AddListener(TetrisState.Move, () => _turnButton.interactable = false);
        
        /*if (newState == TetrisState.MergeElement;
            _turnButton.interactable = false;
        else if (newState == TetrisState.GenerateElement)
            _turnButton.interactable = true;*/
    }
}