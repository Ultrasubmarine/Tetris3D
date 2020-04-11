using System;
using DG.Tweening;
using Script.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovePointUi : MonoBehaviour, IPointerEnterHandler
{
    public event Action<move> onPointEnter;
    public RectTransform _rectTransform { get; private set; }
    
    public move direction { get; private set; }

    [SerializeField] private RectTransform _oreol;

    private Color _startColor;
    
    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _startColor = _oreol.GetComponent<Image>().color;
    }

    public void SetIndex(int i)
    {
        direction = (move)i;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _oreol.DOScale(3, 0.3f);
        _oreol.GetComponent<Image>().DOFade(0, 0.3f).OnComplete(() =>
            {
                onPointEnter?.Invoke(direction);
            });
        
    }

    public void ReturnStartParametrs()
    {
        _oreol.transform.localScale = Vector3.one *0.55f;
        _oreol.GetComponent<Image>().color = _startColor;
    }
    
}
