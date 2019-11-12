using Helper.Patterns.Messenger;
using Script.GameLogic.TetrisElement;
using Script.StateMachine.States;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Turning _Turning;

    // таблица для перемещения блоков в зависимости от угла обзора.
    private move[] A = {move._z, move._x, move.z, move.x};
    private move[] S = {move.x, move._z, move._x, move.z};
    private move[] D = {move.z, move.x, move._z, move._x};
    private move[] W = {move._x, move.z, move.x, move._z};
    private int _indexTable;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры

    public static bool MoveTutorial { get; set; }
    public static bool TurnTutorial { get; set; }

    private void Start()
    {
        Messenger<ETouсhSign>.AddListener( TouchControl.SWIPE, Move);
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

    private void Move(ETouсhSign touch)
    {
        if (Equals(ElementData.NewElement))
            return;

        switch (touch)
        {
            case ETouсhSign.Swipe_LeftUp:
            {
                InfluenceData.direction = A[_indexTable];
                break;
            }
            case ETouсhSign.Swipe_LeftDown:
            {
                InfluenceData.direction = S[_indexTable];
                break;
            }
            case ETouсhSign.Swipe_RightDown:
            {
                InfluenceData.direction = D[_indexTable];
                break;
            }
            case ETouсhSign.Swipe_RightUp:
            {
                InfluenceData.direction = W[_indexTable];
                break;
            }
        }
        var fsm = RealizationBox.Instance.FSM;
        if(fsm.GetCurrentState() == TetrisState.WaitInfluence)
            fsm.SetNewState(TetrisState.Move);
    }

    private void CorrectIndex(int degree)
    {
        _rotY += degree;
        if (_rotY > 360 || _rotY < -360)
            _rotY = 0;

        if (_rotY > -1)
            _indexTable = (int) _rotY / 90;
        else
            _indexTable = ((int) _rotY + 360) / 90;
    }

    private void ResetRotation()
    {
        _rotY = 0;
    }
}