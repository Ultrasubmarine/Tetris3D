using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour {
    [SerializeField] private PlaneScript MyPlane;
    [SerializeField] private Camera MyCamera;
    [SerializeField] private GameObject ObjectLook;

    private GameCameraScript _myCamera;

    // таблица для перемещения блоков в зависимости от угла обзора.
    private move[] A = {move._z, move._x, move.z, move.x};

    private move[] S = {move.x, move._z, move._x, move.z};
    private move[] D = {move.z, move.x, move._z, move._x};
    private move[] W = {move._x, move.z, move.x, move._z};
    private int _indexTable;

    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры

    private void Start() {
        _offset = MyPlane.transform.position -
                  MyCamera.transform.position; // сохраняем расстояние между камерой и полем
        MyCamera.transform.LookAt(ObjectLook.transform.position);
        _myCamera = MyCamera.GetComponent<GameCameraScript>();
    }

    private void LateUpdate() {
        // поворот сцены влево
        if (TouchControll.TouchEvent == touсhSign.LeftOneTouch) {
            //(Input.GetKeyDown(KeyCode.LeftArrow))
            // если поворачивать камеру можно
            if (MyPlane.TurnElement(turn.left)) {
                StartCoroutine(_myCamera.TurnCamera(turn.left, MyPlane.TimeRotation)); //turnCamera(turn.left));
                _rotY += 90;
                if (_rotY == 360 || _rotY == -360)
                    _rotY = 0;

                if (_rotY > -1)
                    _indexTable = (int) _rotY / 90;
                else
                    _indexTable = (int) _rotY + 360 / 90;
            }
        }

        // поворот сцены вправо
        if (TouchControll.TouchEvent == touсhSign.RightOneTouch) {
            // (Input.GetKeyDown(KeyCode.RightArrow))
            if (MyPlane.TurnElement(turn.right)) {
                StartCoroutine(_myCamera.TurnCamera(turn.right, MyPlane.TimeRotation)); //turnCamera(turn.right));
                _rotY += -90;
                if (_rotY == 360 || _rotY == -360)
                    _rotY = 0;

                if (_rotY > -1)
                    _indexTable = (int) _rotY / 90;
                else
                    _indexTable = ((int) _rotY + 360) / 90;
            }
        }

        if (TouchControll.TouchEvent == touсhSign.Swipe_LeftUp) {
            //(Input.GetKeyDown(KeyCode.A))
            // Debug.Log(" A A A ");
            MyPlane.MoveElement(A[_indexTable]);
        }

        if (TouchControll.TouchEvent == touсhSign.Swipe_LeftDown) {
            // (Input.GetKeyDown(KeyCode.S))
            //Debug.Log(" S S S ");
            MyPlane.MoveElement(S[_indexTable]);
        }

        if (TouchControll.TouchEvent == touсhSign.Swipe_RightDown) {
            //(Input.GetKeyDown(KeyCode.D))
            // Debug.Log(" D D D ");
            MyPlane.MoveElement(D[_indexTable]);
        }

        if (TouchControll.TouchEvent == touсhSign.Swipe_RightUp) {//(Input.GetKeyDown(KeyCode.W))
            // Debug.Log(" W W W ");
            MyPlane.MoveElement(W[_indexTable]);
        }

        // обнуляем переменную поскольку мы уже все определили
        TouchControll.TouchEvent = touсhSign.empty;
    }

    private IEnumerator TurnCamera(turn direction) {
        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        // начальный и конечный поворот
        Quaternion rotationStart = Quaternion.Euler(0, _rotY, 0);
        _rotY += angle;
        Quaternion rotationFin = Quaternion.Euler(0, _rotY, 0);

        float countTime = 0;
        while (countTime < MyPlane.TimeRotation) {
            if (countTime + Time.deltaTime < MyPlane.TimeRotation)
                countTime += Time.deltaTime;
            else
                countTime = MyPlane.TimeRotation;

            MyCamera.transform.position = MyPlane.transform.position - (Quaternion.LerpUnclamped(
                                                                            rotationStart,
                                                                            rotationFin,
                                                                            countTime / MyPlane.TimeRotation) *
                                                                        _offset);
            MyCamera.transform.LookAt(ObjectLook.transform.position);

            yield return null; // new WaitForSeconds(MyPlane.TimeRotate / countIter);
        }

        if (_rotY == 360 || _rotY == -360)
            _rotY = 0;

        // Debug.Log("Camera rotate end  = " + (Time.time - fff) );
    }
}