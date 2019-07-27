using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum touсhSign {
    empty,

    // swipe
    Swipe_LeftUp,
    Swipe_LeftDown,
    Swipe_RightUp,
    Swipe_RightDown,

    // touch
    LeftOneTouch,
    RightOneTouch,
}

public class TouchControll : MonoBehaviour {
    /*TODO чекнуть если ли ссылки в инспекторе и переименовать :D*/

    public static touсhSign TouchEvent;

    Vector3 _fp;
    Vector3 _lp;
    readonly float _minSwipeLength = (float) Screen.height * 10 / 100;

    bool _isTouchUI;

    void Update() {
        /* TODO Я бы переписал*/
        if (Input.touchCount == 1) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    Debug.Log(" use UI");
                    _isTouchUI = true;
                }
                else
                    _fp = Input.touches[0].position;

                // Debug.Log(" BEGIN TOUCH");
                // Debug.Log(Input.touches[0].position);
            }
            else if (Input.touches[0].phase == TouchPhase.Moved) {
                // Debug.Log(" MOVED");
            }
            else if (Input.touches[0].phase == TouchPhase.Ended) {
                if (_isTouchUI) {
                    _isTouchUI = !_isTouchUI;
                }
                else {
                    _lp = Input.touches[0].position;
                    //Debug.Log(" END TOUCH");
                    //Debug.Log(Input.touches[0].position);
                    CheckSwipe();
                }
            }
        }
    }

    void CheckSwipe() {
        // длина свайпа
        float swipeLength = Mathf.Sqrt(Mathf.Pow(_lp.x - _fp.x, 2) + Mathf.Pow(_lp.y - _fp.y, 2)); // scrt( ( x2- x1)^2 + ( y2- y1)^2 )

        if (swipeLength < _minSwipeLength) {
            //    Debug.Log(" this touch < 10%");
            TouchEvent = _lp.x < Screen.width / 2 ? touсhSign.LeftOneTouch : touсhSign.RightOneTouch;
            return;
        }

        // отбрасываем перпендикулярные свайпы 
        float cos = Mathf.Abs(_lp.x - _fp.x) / swipeLength;
        //Debug.Log("cos = " + cos);
        if (cos < 0.1 || cos > 0.9) {
            // /*    Debug.Log("PARALLEL")*/;
            return;
        }

        //right sector 
        if (_lp.x > _fp.x) {
            //    Debug.Log(" right swipe");
            TouchEvent = _lp.y < _fp.y ? touсhSign.Swipe_RightDown : touсhSign.Swipe_RightUp;
        }

        //left sector
        else if (_lp.x < _fp.x) {
            //   Debug.Log(" left swipe");
            TouchEvent = _lp.y < _fp.y ? touсhSign.Swipe_LeftDown : touсhSign.Swipe_LeftUp;
        }
    }
}