using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControllerScript : MonoBehaviour
{
    [SerializeField] StateMachine _StateMachine;

    [SerializeField] Moving moving;
    [SerializeField] Turning turning;

    // таблица для перемещения блоков в зависимости от угла обзора.
    private move[] A = { move._z, move._x, move.z, move.x };
    private move[] S = { move.x, move._z, move._x, move.z };
    private move[] D = { move.z, move.x, move._z, move._x };
    private move[] W = { move._x, move.z, move.x, move._z };
    private int indexTable;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY;  // поворот камеры

    static public bool MoveTutorial { get; set; } 
    static public bool TurnTutorial { get; set; } 

    private void Start()
    {
        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.Move, Popo);
    }

    // Update is called once per frame
    private void Update()
    {

        // поворот сцены влево
        if ( TouchControll.TouchEvent == touсhSign.LeftOneTouch && ElementManager.NewElement !=null)//(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if( turning.Action(ElementManager.NewElement, turn.left, Speed.TimeRotate))
                CorrectIndex(90);           
        }
        // поворот сцены вправо
        if (TouchControll.TouchEvent == touсhSign.RightOneTouch && ElementManager.NewElement != null)// (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(turning.Action(ElementManager.NewElement, turn.right, Speed.TimeRotate))
                CorrectIndex(-90);           
        }

        if(TouchControll.TouchEvent == touсhSign.Swipe_LeftUp)//(Input.GetKeyDown(KeyCode.A))
        {
            moving.Action(ElementManager.NewElement, A[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", A[0]);
        }
        if (TouchControll.TouchEvent == touсhSign.Swipe_LeftDown)// (Input.GetKeyDown(KeyCode.S))
        {
            moving.Action(ElementManager.NewElement, S[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", S[0]);
        }
        if (TouchControll.TouchEvent == touсhSign.Swipe_RightDown)//(Input.GetKeyDown(KeyCode.D))
        {
            moving.Action(ElementManager.NewElement, D[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", D[0]);
        }
        if (TouchControll.TouchEvent == touсhSign.Swipe_RightUp)//(Input.GetKeyDown(KeyCode.W))
        {
            moving.Action(ElementManager.NewElement, W[indexTable], Speed.TimeMove);
            if (MoveTutorial)
                Messenger<move>.Broadcast("MOVE", W[0]);
        }

        // обнуляем переменную поскольку мы уже все определили
        TouchControll.TouchEvent = touсhSign.empty;
    }

    public void CorrectIndex( int degree) {
        _rotY += degree;
        if (_rotY == 360 || _rotY == -360)
            _rotY = 0;

        if (_rotY > -1)
            indexTable = (int)_rotY / 90;
        else
            indexTable = ((int)_rotY + 360) / 90;
    }
    
    public void Popo() {

    }
}