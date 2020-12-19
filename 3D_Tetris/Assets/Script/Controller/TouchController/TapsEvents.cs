using System;
using Script.Controller.TouchController;
using UnityEngine.EventSystems;
using UnityEngine;


public enum TouchEventType
{
    None,
    AnalyzingTap,
    SingleTap,
    DoubleTap,
    
    IslandDrag,
}

public enum SwipeDirection
{
    Left,
    Right,
}

public enum BlockingType
{
    None,
    OnlySingleTap,
    SingleAndDouble,
}

public class TapsEvents : MonoBehaviour, IPointerDownHandler, IPointerExitHandler {
 
    // You can add listeners in inspector
    public  event Action OnSingleTap;
    public event Action OnDoubleTap;
 
    public event Action OnDragIceIsland;
    public event Action<SwipeDirection> OnSwipe;
    
    float firstTapTime = 0f;
    float timeBetweenTaps = 0.2f; // time between taps to be resolved in double tap
    float timeSwipe = 0.2f;
    
    private TouchEventType _touchType;
    bool doubleTapInitialized;

    private float _deltaPosition = (Screen.width * 5 / 100);
    private Vector2 _lastPosition;

    public BlockingType _blockTapEvents = BlockingType.None;
    
  private void Awake()
  {
      _touchType = TouchEventType.None;
  }

  public void OnPointerDown(PointerEventData eventData)
    {
        _touchType = TouchEventType.AnalyzingTap;

        _lastPosition = Input.mousePosition;
        
        // invoke single tap after max time between taps
        Invoke("SingleTap", timeBetweenTaps);
 
        if (!doubleTapInitialized)
        {
            // init double tapping
            doubleTapInitialized = true;
            firstTapTime = Time.time;
        }
        else if (Time.time - firstTapTime < timeBetweenTaps)
        {
            // here we have tapped second time before "single tap" has been invoked
            CancelInvoke("SingleTap"); // cancel "single tap" invoking
            DoubleTap();
        }
    }

    private void Update()
    {
        if (_touchType == TouchEventType.AnalyzingTap)
        {
            if (Mathf.Abs(Input.mousePosition.x - _lastPosition.x) > _deltaPosition) // something with Island
            {
                IslandDrag();
            }
        }
    }

    void SingleTap()
    {
        doubleTapInitialized = false; // deinit double tap
        
        if (_touchType == TouchEventType.IslandDrag)
            return;
        
        _lastPosition = Input.mousePosition;
        // fire OnSingleTap event for all eventual subscribers
        if(OnSingleTap != null)
        {
            _touchType = TouchEventType.SingleTap;
            
            OnSingleTap.Invoke();
        }
    }
 
    void DoubleTap()
    {
        doubleTapInitialized = false;
        _touchType = TouchEventType.DoubleTap;
        if(OnDoubleTap != null)
        {
            if(_blockTapEvents != BlockingType.OnlySingleTap)
                OnDoubleTap.Invoke();
        }
    }

    void IslandDrag()
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Island"))
            {
                if (_blockTapEvents == BlockingType.None)
                {
                    _touchType = TouchEventType.IslandDrag;
                    OnDragIceIsland?.Invoke();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      //  throw new NotImplementedException();
    }
}
