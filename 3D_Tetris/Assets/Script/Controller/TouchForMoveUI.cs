using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchForMoveUI : MonoBehaviour
{
    [SerializeField] private TouchForMove _touchForMove;

    [SerializeField] private float _radius  = Screen.width * 0.5f;

    [SerializeField] private List<Image> _images;


    [SerializeField] private Canvas _canvas;

    [SerializeField] private RectTransform _center;
    
    private Vector2 _deltaSize;
    
    private void Awake()
    {
        _touchForMove.onStateChanged += TouchStateChange;
        _deltaSize = _canvas.GetComponent<RectTransform>().sizeDelta;
    }

    private void TouchStateChange( TouchForMove.StateTouch lastState, TouchForMove.StateTouch newState )
    {
        ShowParticals();
        switch (newState)
        {
            case TouchForMove.StateTouch.timeOpen:
                ShowParticals();
                break;    
        }
    }

    private void CreatePoints()
    {
        var mainPoint = Input.GetTouch(0).position;
        
    //    angle 
        
    }
    
    private void ShowParticals()
    {
        const float angle = 40f;
        var center = Input.GetTouch(0).position;
        
        _center.position = new Vector2((center.x ) * _deltaSize.x / Screen.width, (center.y) * _deltaSize.y / Screen.height);
   
        for (int i = 0; i < 4; i++)
        {
            float ang;
            if (i % 2 == 0)
                ang = Mathf.PI * (i / 2) + angle * Mathf.Deg2Rad;
            else
                ang = Mathf.PI * ((i+1) / 2) - angle * Mathf.Deg2Rad;
            
            var pos = new Vector2(center.x + _radius * Mathf.Cos(ang) * _deltaSize.x / Screen.width, 
                (center.y + _radius * Mathf.Sin(ang) * _deltaSize.y / Screen.height));
            
            _images[i].GetComponent<RectTransform>().position = pos ;
        }
    }
}
