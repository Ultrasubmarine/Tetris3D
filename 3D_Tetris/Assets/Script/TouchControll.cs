﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum touсhSign
{
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

    public static touсhSign TouchEvent;

    Vector3 _fp;
    Vector3 _lp;
    float minDelta;

    bool touchUI;
    // Use this for initialization
    void Start() {

        minDelta = Screen.height * 10 / 100;
    }

    // Update is called once per frame
    void Update() {
        
        if (Input.touchCount == 1)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                
                if( EventSystem.current.IsPointerOverGameObject() )
                {
                    Debug.Log(" use UI");
                    touchUI = true;
                }
                else
                    _fp = Input.touches[0].position;
               // Debug.Log(" BEGIN TOUCH");
               // Debug.Log(Input.touches[0].position);
            }
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {
               // Debug.Log(" MOVED");
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                if (touchUI)
                {
                    touchUI = !touchUI;
                }
                else
                {
                    _lp = Input.touches[0].position;
                    //Debug.Log(" END TOUCH");
                    //Debug.Log(Input.touches[0].position);
                    CheckSwipe();
                }
            }

        }
    }

    void CheckSwipe()
    {
        // длина свайпа
        float d = Mathf.Sqrt(Mathf.Pow((_lp.x - _fp.x), 2) + Mathf.Pow((_lp.y - _fp.y), 2));// scrt( ( x2- x1)^2 + ( y2- y1)^2 )

        if (d < minDelta)
        {
        //    Debug.Log(" this touch < 10%");
            if (_lp.x < Screen.width / 2)
                TouchEvent = touсhSign.LeftOneTouch;
            else
                TouchEvent = touсhSign.RightOneTouch;
            return;
        }

        // отбрасываем перпендикулярные свайпы 
        float cos = Mathf.Abs(_lp.x - _fp.x) / d;
        //Debug.Log("cos = " + cos);
        if (cos < 0.1 || cos > 0.9)
        {
       // /*    Debug.Log("PARALLEL")*/;
            return;
        }

        //right sector 
        if (_lp.x > _fp.x)
        {
        //    Debug.Log(" right swipe");
            if (_lp.y < _fp.y)
            {
             //   Debug.Log("Down");
                TouchEvent = touсhSign.Swipe_RightDown;
            }
            else
            {
             //   Debug.Log("Up");
                TouchEvent = touсhSign.Swipe_RightUp;
            }     
        }

        //left sector
        else if (_lp.x < _fp.x)
        {
         //   Debug.Log(" left swipe");
            if (_lp.y < _fp.y)
            {
             //   Debug.Log("Down");
                TouchEvent = touсhSign.Swipe_LeftDown;
            }
            else
            {
             //   Debug.Log("Up");
                TouchEvent = touсhSign.Swipe_LeftUp;
            }
        }

    }
}