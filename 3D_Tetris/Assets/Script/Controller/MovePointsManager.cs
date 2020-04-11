using System.Collections.Generic;
using UnityEngine;

public class MovePointsManager : MonoBehaviour
{
    public List<MovePointUi> points => _points;
    
    [SerializeField] private float _radius  = Screen.width * 0.5f;
    
    [SerializeField] private List<MovePointUi> _points;

    [SerializeField] private Canvas _canvas;
    
    [SerializeField] private RectTransform _center;
    
    private Vector2 _deltaSize;
    
    private void Awake()
    {
        _deltaSize = _canvas.GetComponent<RectTransform>().sizeDelta;
        
        for (var i = 0; i < _points.Count; i++)
        {
            _points[i].SetIndex(i);
        }
    }
    
    public void ShowPoints()
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
            
            _points[i].GetComponent<RectTransform>().position = pos ;
        }
    }
    
}
