using DG.Tweening;
using Helper.Patterns.Messenger;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Turning _Turning;

    [SerializeField] private float _timeTurn = 0.3f;
    
    // таблица для перемещения блоков в зависимости от угла обзора.
    /*private move[] left_up = {move._z, move._x, move.z, move.x};
    private move[] left_down = {move.x, move._z, move._x, move.z};
    private move[] right_down = {move.z, move.x, move._z, move._x};
    private move[] right_up = {move._x, move.z, move.x, move._z};*/
    
    private move[] left_up = {move._z, move.x, move.z, move._x};
    private move[] left_down = {move.x, move.z, move._x, move._z};
    private move[] right_down = {move.z, move._x, move._z, move.x};
    private move[] right_up = {move._x, move._z, move.x, move.z};
    private int _indexTable = 0;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры
    private int _rotate = 0;
    private Transform _place;
    private bool _isTurn = false;
    
    public static bool MoveTutorial { get; set; }
    public static bool TurnTutorial { get; set; }

    private PointJoystick _pointJoystick;
    
    private void Start()
    {
        Messenger<ETouсhSign>.AddListener( TouchControl.SWIPE, Move);

        _pointJoystick = RealizationBox.Instance.pointJoystick;
        _pointJoystick.onPointEnter += Move;
//        Messenger<ETouсhSign>.AddListener( TouchControl.ONE_TOUCH, Turn);
//            
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ResetRotation);
    }

    private void OnDestroy()
    {
//        Messenger<ETouсhSign>.RemoveListener( TouchControl.SWIPE, Move);
//        Messenger<ETouсhSign>.RemoveListener( TouchControl.ONE_TOUCH, Turn);
//        
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ResetRotation);
    }

    /*
    private void Turn(ETouсhSign touch)
    {
        if (Equals(ElementData.NewElement))
            return;

        if (touch == ETouсhSign.OneTouch_Left)
        {
            if (_Turning.Action(ElementData.NewElement, turn.left, Speed.TimeRotate))
                CorrectIndex(90);
        }
        else
        {
            //ETouсhSign.OneTouch_Right
            if (_Turning.Action(ElementData.NewElement, turn.right, Speed.TimeRotate))
                CorrectIndex(90);
        }
    }
*/

    public void Move(ETouсhSign touch)
    {
//        if (Equals(ElementData.NewElement))
//            return;
//
//        switch (touch)
//        {
//            case ETouсhSign.Swipe_LeftUp:
//            {
//                InfluenceData.direction = A[_indexTable];
//                break;
//            }
//            case ETouсhSign.Swipe_LeftDown:
//            {
//                InfluenceData.direction = S[_indexTable];
//                break;
//            }
//            case ETouсhSign.Swipe_RightDown:
//            {
//                InfluenceData.direction = D[_indexTable];
//                break;
//            }
//            case ETouсhSign.Swipe_RightUp:
//            {
//                InfluenceData.direction = W[_indexTable];
//                break;
//            }
//        }
//        var fsm = RealizationBox.Instance.FSM;
//        if(fsm.GetCurrentState() == TetrisState.WaitInfluence)
//            fsm.SetNewState(TetrisState.Move);
    }
    
    public void Move(move touch)
    {
        if (Equals(ElementData.NewElement))
            return;

        Debug.Log("touch " );
        switch (touch)
        {
            case move._z:
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
            case move._x:
            {
                InfluenceData.direction = right_up[_indexTable];
                break;
            }
        }
        var fsm = RealizationBox.Instance.FSM;
        if(fsm.GetCurrentState() == TetrisState.WaitInfluence)
            fsm.SetNewState(TetrisState.Move);
    }
    

    private void CorrectIndex()
    {
        _rotate = (_rotate + 90) % 360;
        _indexTable = (_indexTable + 1) % 4;
    //    _indexTable = ((int) _rotate + 360) / 90;
     //   if (_rotate > -1)
        //    _indexTable = (int) _rotate / 90;
        /*else
            _indexTable = ((int) _rotate + 360) / 90;*/
    }

    private void ResetRotation()
    {
        _rotY = 0;
    }

    public void Turn()
    {
        if (_isTurn)
            return;
        
        _isTurn = true;
        var _influence = RealizationBox.Instance.influenceManager;
        _place = RealizationBox.Instance.place;
        _influence.enabled = false;

      // _rotate = (_rotate + 90) % 360;
       CorrectIndex();
        _place.DORotate(new Vector3(0, _rotate, 0), _timeTurn).OnComplete( () =>
        {
            _influence.enabled = true;
            _isTurn = false;
        });
        
        /*
              
               var fsm = RealizationBox.Instance.FSM;
               if(fsm.GetCurrentState() != TetrisState.Turn)
                   fsm.SetNewState(TetrisState.Turn);*/
    }
}