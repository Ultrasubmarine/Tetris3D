using UnityEngine;
using UnityEngine.UI;

namespace Script.Booster
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(CanvasGroup))]
    public class BoosterUi : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _fill;
        
        private Button _button;

        private BoosterBase _booster;

        private CanvasGroup _canvasGroup;
        
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Initialize(BoosterBase booster)
        {
            _booster = booster;
            
            _booster.onStateChange += OnBoosterStateChange;
            _icon.sprite = _booster.icon;
            _button.onClick.AddListener(_booster.Apply);
        }

        private void OnBoosterStateChange(BoosterState newState)
        {
            switch (newState)
            {
                case BoosterState.ReadyForUse:
                {
                    _button.enabled = true;
                    _canvasGroup.alpha = 1f;
                    _fill.fillAmount = 1;
                    break;
                }
                case BoosterState.Respawn:
                {
                    _booster.timer.onProgressChanged += (p) => { _fill.fillAmount = p; };
                    _button.enabled = false;
                    _canvasGroup.alpha = 0.5f;
                    break;
                }
                case BoosterState.UseWithCountdown: 
                {
                    _booster.useTimer.onProgressChanged += (p) => { _fill.fillAmount = 1 - p; };
                    _button.enabled = false;
                    _canvasGroup.alpha = 1f;
                    break;
                }
            }
        }
    }
}