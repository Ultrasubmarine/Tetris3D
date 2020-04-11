using System;
using Script.Controller;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePointUi : MonoBehaviour, IPointerEnterHandler
{
    public event Action<move> onPointEnter;
    public RectTransform _rectTransform { get; private set; }
    
    public move direction { get; private set; }

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetIndex(int i)
    {
        direction = (move)i;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointEnter?.Invoke(direction);
        Debug.Log("Hand me");
    }
    
}
