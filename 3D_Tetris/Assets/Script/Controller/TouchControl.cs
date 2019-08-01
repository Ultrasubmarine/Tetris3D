using System;
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

public class TouchControl : MonoBehaviour {

    public static touсhSign TouchEvent;

    Vector3 _fp;
    Vector3 _lp;
    readonly float _minSwipeLength = (float) Screen.height * 10 / 100;

    bool _isTouchUI;

    private float _halfWight;

    void Awake() {
        _halfWight = Screen.width / 2;
    }

    void Update() {
        if (Input.touchCount != 1) return;

        switch (Input.touches[0].phase) {
            case TouchPhase.Began: {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    _isTouchUI = true;
                }
                else
                    _fp = Input.touches[0].position;

                break;
            }
            case TouchPhase.Ended: {
                if (_isTouchUI) {
                    _isTouchUI = !_isTouchUI;
                }
                else {
                    _lp = Input.touches[0].position;
                    CheckSwipe();
                }

                break;
            }
            default:
                break;
        }
    }

    void CheckSwipe() {
 
        float swipeLength = Mathf.Sqrt(Mathf.Pow(_lp.x - _fp.x, 2) + Mathf.Pow(_lp.y - _fp.y, 2)); // scrt( ( x2- x1)^2 + ( y2- y1)^2 )

        if (swipeLength < _minSwipeLength) {
            TouchEvent = _lp.x < _halfWight ? touсhSign.LeftOneTouch : touсhSign.RightOneTouch;
            return;
        }

        // отбрасываем перпендикулярные свайпы 
        float cos = Mathf.Abs(_lp.x - _fp.x) / swipeLength;

        if (cos < 0.1 || cos > 0.9) {
            return;
        }

        //right sector 
        if (_lp.x > _fp.x) {
            TouchEvent = _lp.y < _fp.y ? touсhSign.Swipe_RightDown : touсhSign.Swipe_RightUp;
        }

        //left sector
        else if (_lp.x < _fp.x) {
            TouchEvent = _lp.y < _fp.y ? touсhSign.Swipe_LeftDown : touсhSign.Swipe_LeftUp;
        }
    }
}