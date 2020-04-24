using Script.Influence;
using UnityEngine;

namespace Script.UI
{
    public class PausePanel : MonoBehaviour
    {
        private Animator _animator;
        
        private const string _show = "Show";
    
        private const string _hide = "Hide";

        private InfluenceManager _influenceManager;
        
        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Start()
        {
            _influenceManager = RealizationBox.Instance.influenceManager;
        }

        public void SetPauseGame(bool isPause)
        {
            gameObject.SetActive(!isPause);
            _influenceManager.enabled = !isPause;
            
            if(isPause)
                Show();
            else
                Hide();
        }
        
        public void DisablePanel()
        {
            gameObject.SetActive(false);
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            _animator.Play(_show);    
        }

        private void Hide()
        {
            _animator.Play(_hide);
        }
    }
}