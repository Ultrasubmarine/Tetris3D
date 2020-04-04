using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePointUi : MonoBehaviour, IPointerEnterHandler
{
    public event Action onPointEnter;
    public RectTransform _rectTransform { get; private set; }
    
    public int index { get; private set; }

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
    }

    public void SetIndex(int i)
    {
        index = i;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointEnter?.Invoke();
        Debug.Log("Hand me");
    }
    
}
