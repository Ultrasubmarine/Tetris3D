using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] StateMachine _StateMachine;

    [SerializeField] Moving _Moving;
    [SerializeField] Turning _Turning;

    // таблица для перемещения блоков в зависимости от угла обзора.
    private move[] A = { move._z, move._x, move.z, move.x };
    private move[] S = { move.x, move._z, move._x, move.z };
    private move[] D = { move.z, move.x, move._z, move._x };
    private move[] W = { move._x, move.z, move.x, move._z };
    private int _indexTable;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY;  // поворот камеры

    static public bool MoveTutorial { get; set; } 
    static public bool TurnTutorial { get; set; } 

    private void Start()
    {
        Messenger<ETouсhSign>.AddListener( TouchControl.SWIPE, Move);
        Messenger<ETouсhSign>.AddListener( TouchControl.ONE_TOUCH, Turn);
            
        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ResetRotation);
    }

    void OnDestroy()
    {
        Messenger<ETouсhSign>.RemoveListener( TouchControl.SWIPE, Move);
        Messenger<ETouсhSign>.RemoveListener( TouchControl.ONE_TOUCH, Turn);
        
        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ResetRotation);
    }

    void Turn(ETouсhSign touch)
    {
        if (Equals( ElementManager.NewElement) )
            return;

        if(touch == ETouсhSign.OneTouch_Left) {
            if( _Turning.Action(ElementManager.NewElement, turn.left, Speed.TimeRotate))
                CorrectIndex(90);             
        }
        else { //ETouсhSign.OneTouch_Right
            if( _Turning.Action(ElementManager.NewElement, turn.right, Speed.TimeRotate))
                CorrectIndex(90);    
        }
    }

    void Move( ETouсhSign touch) {
        
        if (Equals( ElementManager.NewElement) )
            return;

        switch (touch)
        {
            case ETouсhSign.Swipe_LeftUp:
            {
                _Moving.Action(ElementManager.NewElement, A[_indexTable], Speed.TimeMove);
                break;
            }
            case ETouсhSign.Swipe_LeftDown:
            {
                _Moving.Action(ElementManager.NewElement, S[_indexTable], Speed.TimeMove);
                break;
            }
            case ETouсhSign.Swipe_RightDown:
            {
                _Moving.Action(ElementManager.NewElement, D[_indexTable], Speed.TimeMove);
                break;
            }
            case ETouсhSign.Swipe_RightUp:
            {
                _Moving.Action(ElementManager.NewElement, W[_indexTable], Speed.TimeMove);
                break;
            }
        }
    }
    
    private void CorrectIndex( int degree) {
        _rotY += degree;
        if (_rotY > 360 || _rotY < -360)
            _rotY = 0;

        if (_rotY > -1)
            _indexTable = (int)_rotY / 90;
        else
            _indexTable = ((int)_rotY + 360) / 90;
    }

    void ResetRotation() {
        _rotY = 0;
    }
}