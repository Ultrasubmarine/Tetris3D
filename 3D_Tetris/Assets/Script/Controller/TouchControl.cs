using UnityEngine;

public enum ETouсhSign {
    empty,

    // swipe
    Swipe_LeftUp,
    Swipe_LeftDown,
    Swipe_RightUp,
    Swipe_RightDown,

    // touch
    OneTouch_Left,
    OneTouch_Right,
}


public class TouchControl : MonoBehaviour {

     ETouсhSign _touchEvent;

    Vector3 _fp;
    Vector3 _lp;
    readonly float _minSwipeLength = (float) Screen.height * 10 / 100;

    bool _isTouchUI;

    private float _halfWight;    
    private enum ETouchType
    {
        OneClick,
        Swipe,
    }
    
    public static readonly string ONE_TOUCH = ETouchType.OneClick.ToString();
    public static readonly string SWIPE = ETouchType.Swipe.ToString();
    
    void Awake() {
        _halfWight = Screen.width / 2;
    }

    void Update() {
        if (Input.touchCount != 1) return;

        switch (Input.touches[0].phase) {
            case TouchPhase.Began: {
                _fp = Input.touches[0].position;
                break;
            }
            case TouchPhase.Ended: {
                _lp = Input.touches[0].position;
                CheckSwipe();
                break;
            }
            default:
                break;
        }
    }

    void CheckSwipe() {
 
        float swipeLength = Mathf.Sqrt(Mathf.Pow(_lp.x - _fp.x, 2) + Mathf.Pow(_lp.y - _fp.y, 2)); // scrt( ( x2- x1)^2 + ( y2- y1)^2 )

        if (swipeLength < _minSwipeLength) {
            _touchEvent = _lp.x < _halfWight ? ETouсhSign.OneTouch_Left : ETouсhSign.OneTouch_Right;
            Messenger<ETouсhSign>.Broadcast(ONE_TOUCH, _touchEvent);
            return;
        }

        // отбрасываем перпендикулярные свайпы 
        float cos = Mathf.Abs(_lp.x - _fp.x) / swipeLength;

        if (cos < 0.1 || cos > 0.9) {
            return;
        }

        //right sector 
        if (_lp.x > _fp.x) {
            _touchEvent = _lp.y < _fp.y ? ETouсhSign.Swipe_RightDown : ETouсhSign.Swipe_RightUp;
        }

        //left sector
        else if (_lp.x < _fp.x) {
            _touchEvent = _lp.y < _fp.y ? ETouсhSign.Swipe_LeftDown : ETouсhSign.Swipe_LeftUp;
        }
        
        Messenger<ETouсhSign>.Broadcast(SWIPE, _touchEvent);
    }
}