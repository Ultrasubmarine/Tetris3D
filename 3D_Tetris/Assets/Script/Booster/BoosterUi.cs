using UnityEngine;
using UnityEngine.UI;

namespace Script.Booster
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class BoosterUi : MonoBehaviour
    {
        private Image _icon;

        private Button _button;

        private BoosterBase _booster;
        private void Awake()
        {
            _icon = GetComponent<Image>();
            _button = GetComponent<Button>();
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
                    break;
                }
                case BoosterState.Respawn:
                {
                    _booster.timer.onProgressChanged += (p) => { _icon.fillAmount = p; };
                    _button.enabled = false;
                    break;
                }
            }
        }
    }
}