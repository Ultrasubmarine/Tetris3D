using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Turning : MonoBehaviour{

    // for camera
    private Vector3 _offset; // начальное положение между камерой и площадкой
    private float _rotY; // поворот камеры
    [SerializeField] GameObject _Camera;

    [SerializeField] StateMachine _StateMachine;
    [SerializeField] private GameObject _ObjectLook;

    private PlaneMatrix _matrix;

    private void Start() {
        _offset = Vector3.zero - _Camera.transform.position; // сохраняем расстояние между камерой и полем
        _matrix = PlaneMatrix.Instance;
    }

    public bool Action(Element element, turn direction, float time) {

        if (CheckOpportunity(direction, element)) {

            if (!_StateMachine.ChangeState(GameState2.Turn, false))
                return false;

            Logic(direction, element);
            Vizual(direction, element, time);
            return true;
        }
        return false;
    }

    bool CheckOpportunity(turn direction, Element element) {

        int x, z;
        if (direction == turn.left) {
            foreach (var item in element.MyBlocks) {
                // по правилу поворота
                x = item.Coordinates.z;
                z = -item.Coordinates.x;

                if (_matrix._matrix[x + 1, item.Coordinates.y, z + 1] != null)
                    return false;
            }
        }
        else {
            foreach (var item in element.MyBlocks) {
                // по правилу поворота
                x = -item.Coordinates.z;
                z = item.Coordinates.x;

                if (_matrix._matrix[x + 1, item.Coordinates.y, z + 1] != null)
                    return false;
            }
        }

        return true;        
    }

    void Logic(turn direction, Element element) {
            if (direction == turn.left)
            {
                foreach (Block item in element.MyBlocks) {
                    int temp = item.Coordinates.x;
                    item.SetCoordinates(item.Coordinates.z, item.Coordinates.y, -item.Coordinates.x );
                }
            }
            else {
                foreach (Block item in element.MyBlocks) {
                    int temp = item.Coordinates.x;
                    item.SetCoordinates(-item.Coordinates.z, item.Coordinates.y, item.Coordinates.x );
                }
            }
    }

    void Vizual(turn direction, Element element, float time) {

        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        StartCoroutine(TurnElement(element, angle, time, _ObjectLook));
        StartCoroutine(TurnCamera(direction, time));
    }

    IEnumerator TurnCamera(turn direction, float time) {

        int angle;
        if (direction == turn.left)
            angle = 90;
        else
            angle = -90;

        // начальный и конечный поворот
        Quaternion rotationStart = Quaternion.Euler(0, _rotY, 0);
        _rotY += angle;
        Quaternion rotationEnd = Quaternion.Euler(0, _rotY, 0);

        float countTime = 0;
        while (countTime < time) {
            if ((countTime + Time.deltaTime) < time)
                countTime += Time.deltaTime;
            else
                countTime = time;

            _Camera.transform.position = Vector3.zero -
                                 Quaternion.LerpUnclamped(rotationStart, rotationEnd, countTime / time) * _offset;

            _Camera.transform.LookAt(_ObjectLook.transform.position);
            yield return new WaitForEndOfFrame();
        }

        if (_rotY >= 360 || _rotY <= -360)
            _rotY = 0;

        _StateMachine.ChangeState(GameState2.NewElement, false);
    }

    IEnumerator TurnElement(Element element, int angle, float time, GameObject target) {

        float deltaAngle;
        float countAngle = 0;

        do {
            deltaAngle = angle * (Time.deltaTime / time);
            if (angle > 0 && countAngle + deltaAngle > angle || angle < 0 && countAngle + deltaAngle < angle) // если мы уже достаточно повернули и в ту и в другую сторону
            {
                deltaAngle = angle - countAngle; // узнаем сколько нам не хватает на самом деле  
                countAngle = angle;
            }
            else
                countAngle += deltaAngle;

            element.MyTransform.Rotate(target.transform.position, deltaAngle);

            yield return null;
        } while (angle > 0 && countAngle < angle || angle < 0 && countAngle > angle);
    }
}
