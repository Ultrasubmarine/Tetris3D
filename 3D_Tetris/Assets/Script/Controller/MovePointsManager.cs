using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Controller;
using UnityEngine;
using UnityEngine.UI;

public class MovePointsManager : MonoBehaviour
{
    public event Action<move> onPointEnter;

    [SerializeField] private float _radius  = Screen.width * 0.7f;
    
    [SerializeField] private List<MovePointUi> _points;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private CanvasGroup _pointsParent;

    [SerializeField] private float _timeShowAnimations = 0.2f;

    [SerializeField] private Transform _fakeApply;
    
    private Image _fakeApplyImage;

    private Sequence _clickAnimationSequence;
    
    private Vector2 _deltaSize;

    private bool _applyPointInCycle = false;
    
    private void Awake()
    {
        _deltaSize = _canvas.GetComponent<RectTransform>().sizeDelta;
        _fakeApplyImage = _fakeApply.gameObject.GetComponent<Image>();
        
        for (var i = 0; i < _points.Count; i++)
        {
            _points[i].SetIndex(i);
            _points[i].onPointEnter += OnMovePointUiTouch;
        }

        _clickAnimationSequence = DOTween.Sequence().SetAutoKill(false);

        _clickAnimationSequence.Append(_fakeApply.DOScale(8, 0.3f).From(1.24f, false))
            .Join(_fakeApplyImage.DOFade(0, 0.3f).From(0.26f, false));
    }
    
    public void ShowPoints()
    {
        _applyPointInCycle = false;
        const float angle = 40f;
        var center = Input.GetTouch(0).position;

        for (int i = 0; i < _points.Count; i++)
        {
            float ang;
            if (i % 2 == 0)
                ang = Mathf.PI * (i / 2) + angle * Mathf.Deg2Rad;
            else
                ang = Mathf.PI * ((i+1) / 2) - angle * Mathf.Deg2Rad;
            
            var pos = new Vector2(center.x + _radius * Mathf.Cos(ang) * _deltaSize.x / Screen.width, 
                (center.y + _radius * Mathf.Sin(ang) * _deltaSize.y / Screen.height));

            _points[i].rectTransform.position = center;

            _points[i].enabled = false;
            var point = _points[i];
            _points[i].rectTransform.DOMove(pos, _timeShowAnimations).
                OnComplete(() => {  point.enabled = true; });
        }

        _pointsParent.DOFade(1, _timeShowAnimations);
    }

    public void HidePoints()
    {
        _pointsParent.DOFade(0, _timeShowAnimations);
        
        for (var i = 0; i < _points.Count; i++)
        {
            _points[i].enabled = false;
        }
    }

    private void OnMovePointUiTouch(MovePointUi point)
    {
        if (_applyPointInCycle)
            return;
        
        _applyPointInCycle = true;
        
        _fakeApply.position = point.transform.position;
        _clickAnimationSequence.Rewind();
        _clickAnimationSequence.Play();
        onPointEnter?.Invoke(point.direction);
    }
}
