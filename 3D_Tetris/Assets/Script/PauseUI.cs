using UnityEngine;

public class PauseUI : MonoBehaviour
{
    private Animator _animator;

    private float _lastTimeScale;

    
    private const string _show = "Show";
    
    private const string _hide = "Hide";
    
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Close()
    {
        Time.timeScale = _lastTimeScale;
        gameObject.SetActive(false);
    }

    public void SetPauseGame(bool isPause)
    {
        if(isPause)
            Show();
        else
            Hide();
    }

    
    private void Show()
    {
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0;
        
        gameObject.SetActive(true);
        
        _animator.Play(_show);    
    }

    private void Hide()
    {
        _animator.Play(_hide);
    }
}