using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenChanger : MonoBehaviour
{
    [SerializeField] private Image _screen;
    
    [SerializeField] private float _duration;
    
    [SerializeField] private UnityEvent _onHideScreen;
    
    [SerializeField] private UnityEvent _onEndScreenChange;

    private Sequence _animation;

    [SerializeField] private bool _startWithHide;
    [SerializeField] private bool _startWhenActive;
    private void Awake()
    {
        _animation = DOTween.Sequence().SetAutoKill(false).Pause();

        if (_startWithHide)
        {
            _animation.Append(_screen.DOFade(0, _duration).From(1, false).OnPlay(Hide).OnComplete(EndChange));
        }
        else
        {
            _animation.Append(_screen.DOFade(1, _duration/2).From(0, false).OnComplete(Hide)).
                Append(_screen.DOFade(0, _duration/2).From(1, false).OnComplete(EndChange));
        }
  
    }

    private void Start()
    {
        if(_startWhenActive)
            Play();
    }

    public void Play()
    {
        _animation.Rewind();
        _animation.Play();
    }
    
    public void Hide()
    {
        _onHideScreen.Invoke();
    }

    public void EndChange()
    {
        _onEndScreenChange.Invoke();
    }
}