using System;
using UnityEngine.EventSystems;
using UnityEngine;


public class TapsEvents : MonoBehaviour, IPointerDownHandler {
 
    // You can add listeners in inspector
    public  event Action OnSingleTap;
    public event Action OnDoubleTap;
 
    public event Action OnTurnIceIsland;
 
    float firstTapTime = 0f;
    float timeBetweenTaps = 0.2f; // time between taps to be resolved in double tap
    bool doubleTapInitialized;

    private float _deltaPosition = (Screen.width * 5 / 100);
    private float _lastPosition;

    private bool isAnalyzingTap = false;
    private bool isTurnIsland = false;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isAnalyzingTap = true;
        isTurnIsland = false;
        _lastPosition = Input.mousePosition.x;
        
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
        if (isAnalyzingTap)
        {
            if (Mathf.Abs(Input.mousePosition.x - _lastPosition) > _deltaPosition)
                IslandSwipe();
        }
    }

    void SingleTap()
    {
        doubleTapInitialized = false; // deinit double tap

        if (isTurnIsland)
            return;
        
        _lastPosition = Input.mousePosition.x;
        // fire OnSingleTap event for all eventual subscribers
        if(OnSingleTap != null)
        {
            OnSingleTap.Invoke();
        }

        isAnalyzingTap = false;
    }
 
    void DoubleTap()
    {
        doubleTapInitialized = false;
        if(OnDoubleTap != null)
        {
            OnDoubleTap.Invoke();
        }

        isAnalyzingTap = false;
    }

    void IslandSwipe()
    {
        isTurnIsland = true;
        isAnalyzingTap = false;
        
        OnTurnIceIsland.Invoke();
    }
    
}
