using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Cards
{
    public enum CardState
    {
        locked,
        unlocked,
        current
    }
    
    public class CardIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        [SerializeField] private GameObject _lock;
        [SerializeField] private GameObject _cardHider;
        [SerializeField] private GameObject _picture;

        [SerializeField] private Image _image;
        private CardState _state;
        
        public void SetState(CardState state)
        {
            _state = state;
            switch (state)
            {
                case CardState.locked:
                {
                    _cardHider.SetActive(true);
                    _lock.SetActive(true);
                    _picture.SetActive(false);
                    _text.gameObject.SetActive(false);
                    break;
                }
                case CardState.unlocked:
                {
                    _cardHider.SetActive(false);
                    _lock.SetActive(false);
                    _picture.SetActive(true);
                    _text.gameObject.SetActive(false);
                    break;
                }
                case CardState.current:
                {
                    _cardHider.SetActive(true);
                    _lock.SetActive(false);
                    _picture.SetActive(false);
                    _text.gameObject.SetActive(true);
                    break;
                }
            }
        }

        public void SetPicture(Sprite pic)
        {
            _image.sprite = pic;
        }

        public void SetProgress(string amount)
        {
            _text.text = amount;
        }
    }
}