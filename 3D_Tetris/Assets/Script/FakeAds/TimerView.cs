using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
    public  Timer timer => _timer;
    private Timer _timer;

    [SerializeField] private Image _progress;
    [SerializeField] private float _timeRespawn;
    [SerializeField] private Text _text;
    
    // Start is called before the first frame update
    void Start()
    {
        var t = TimersKeeper.Schedule(_timeRespawn);
        SetTimer(ref t);
    }

    public void SetTimer(ref Timer timer)
    {
        if (_timer != null)
        {
            _timer.onProgressChanged -= SetProgress;
           
        }
        
        _timer = timer;
        _timer.onProgressChanged += SetProgress;
    }

    private void SetProgress(float progress)
    {
        _progress.fillAmount = progress;
        _text.text = _timer.timeLeft.Seconds.ToString();
    }

    public void Reset()
    {
        _progress.fillAmount = 0;
        _text.text = "";
    }
    
}
