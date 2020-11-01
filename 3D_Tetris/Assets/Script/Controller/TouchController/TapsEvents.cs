using System;
using Script.Controller.TouchController;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class TapsEvents : MonoBehaviour, IPointerDownHandler {
 
    // You can add listeners in inspector
    public  event Action OnSingleTap;
    public event Action OnDoubleTap;
 
    public event Action OnTapOnIceIsland;
 
    float firstTapTime = 0f;
    float timeBetweenTaps = 0.2f; // time between taps to be resolved in double tap
    bool doubleTapInitialized;
 
    public void OnPointerDown(PointerEventData eventData)
    {
        
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
 
    void SingleTap()
    {
        doubleTapInitialized = false; // deinit double tap
 
        if (IsIsland())
            return;
        // fire OnSingleTap event for all eventual subscribers
        if(OnSingleTap != null)
        {
            OnSingleTap.Invoke();
        }
    }
 
    void DoubleTap()
    {
        doubleTapInitialized = false;
        if(OnDoubleTap != null)
        {
            OnDoubleTap.Invoke();
        }
    }

    bool IsIsland()
    {
        var mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            return false;

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(mousePos), out var hit))
            return false;

        if (hit.collider.tag == "Island")
        {
            Debug.Log("IS ISLAND");
            this.GetComponent<IslandTurn>().Turn(true);
            return true;
        }
        return false;
    }
}
