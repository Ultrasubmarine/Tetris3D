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
    SingleAndDrag,
}

public class TapsEvents : MonoBehaviour, IPointerDownHandler, IPointerExitHandler {
 
    // You can add listeners in inspector
    public  event Action OnSingleTap; // long
    public event Action OnDoubleTap;
    public event Action OnOneTap;
    
    public event Action OnDragIceIsland;
    public event Action<SwipeDirection> OnSwipe;
    
    float firstTapTime = 0f;
    [SerializeField] float timeBetweenTaps = 0.2f; // time between taps to be resolved in double tap
    [SerializeField] float timeSwipe = 0.2f;
    
    private TouchEventType _touchType;
    bool doubleTapInitialized;

    private float _deltaPosition = (Screen.width * 5 / 100);
    private Vector2 _lastPosition;

    public BlockingType _blockTapEvents = BlockingType.None;

    private int amountTap = 0;
    
  private void Awake()
  {
      _touchType = TouchEventType.None;
  }

  private void OnEnable()
  {
      if (Input.touches.Length == 1)
      {
          OnPointerDown(null);
      }
  }

  public void OnPointerDown(PointerEventData eventData)
  {
        amountTap++;
        _touchType = TouchEventType.AnalyzingTap;

        _lastPosition = Input.mousePosition;

        float waitTime = timeBetweenTaps;
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Island"))
            {
                if (_blockTapEvents == BlockingType.None || _blockTapEvents == BlockingType.SingleAndDrag)
                {
                    waitTime *= 1.15f;
                }
            }
        }
        
        // invoke single tap after max time between taps
        Invoke("SingleTap", waitTime);
 
        if (!doubleTapInitialized)
        {
            // init double tapping
            doubleTapInitialized = true;
            firstTapTime = Time.time;
        }
        else if (Time.time - firstTapTime < waitTime || amountTap > 1)
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
        if (amountTap > 1)
        {
            DoubleTap();
            return;
        }
        
        amountTap = 0;
        if (Input.touchCount == 0) // not work
        {
            _touchType = TouchEventType.None;
            return;
        }
        
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
        amountTap = 0;
        doubleTapInitialized = false;
        _touchType = TouchEventType.DoubleTap;
        if(OnDoubleTap != null)
        {
            if(_blockTapEvents != BlockingType.OnlySingleTap && _blockTapEvents != BlockingType.SingleAndDrag)
                OnDoubleTap.Invoke();
        }
    }

    void IslandDrag()
    {
        amountTap = 0;
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Island"))
            {
                if (_blockTapEvents == BlockingType.None || _blockTapEvents == BlockingType.SingleAndDrag)
                {
                    _touchType = TouchEventType.IslandDrag;
                    OnDragIceIsland?.Invoke();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(amountTap == 1)
            OnOneTap?.Invoke();
        //  throw new NotImplementedException();
    }
}
