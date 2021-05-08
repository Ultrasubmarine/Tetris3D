using System;
using Script.Controller.TouchController;
using Script.GameLogic.TetrisElement;
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
    public  event Action OnSingleTap;
    public event Action OnDoubleTap;
 
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
        _touchType = TouchEventType.AnalyzingTap;
         
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
            CancelInvoke("DragIsland"); // cancel "single tap" invoking
            DoubleTap();
        }
        
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Island"))
            {
                if (_blockTapEvents == BlockingType.None || _blockTapEvents == BlockingType.SingleAndDrag)
                {
                    Invoke("DragIsland", timeBetweenTaps);
                }
            }
            else if (hit.collider.CompareTag(ElementData.NewElementTag))
            {
                DoubleTap();
            }
            else
            {
                Invoke("SingleTap", timeBetweenTaps);
            
            }
        }
        else
        {
            Invoke("SingleTap", timeBetweenTaps);
        }
    }

  void SingleTap()
    {
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
        doubleTapInitialized = false;
        _touchType = TouchEventType.DoubleTap;
        if(OnDoubleTap != null)
        {
            if(_blockTapEvents != BlockingType.OnlySingleTap && _blockTapEvents != BlockingType.SingleAndDrag)
                OnDoubleTap.Invoke();
        }
    }

    void DragIsland()
    {
        doubleTapInitialized = false;
        _touchType = TouchEventType.IslandDrag;
        OnDragIceIsland?.Invoke();
    }
    void IslandDrag()
    {
        doubleTapInitialized = false;
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
        //  throw new NotImplementedException();
    }
}
