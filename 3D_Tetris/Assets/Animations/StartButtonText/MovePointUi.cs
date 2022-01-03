using System;
using Script.Controller;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePointUi : MonoBehaviour, IPointerEnterHandler
{
    public event Action<MovePointUi> onPointEnter;
    public RectTransform rectTransform { get; private set; }
    
    public move direction { get; private set; }
    
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetIndex(int i)
    {
        direction = (move)i;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointEnter?.Invoke(this);
    }
}
