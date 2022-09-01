using System;
using TMPro;
using UnityEditor;
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
        public Action<int> OnButtonClick;
        
        [SerializeField] private TextMeshProUGUI _text;
        
        [SerializeField] private GameObject _lock;
        [SerializeField] private GameObject _cardHider;
        [SerializeField] private GameObject _picture;
        [SerializeField] private Button _button;
        
        [SerializeField] private Image _image;
        public CardState _state;
        private int _index;
        
        private void Start()
        {
            _button.onClick.AddListener(() => OnButtonClick.Invoke(_index));
        }

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
                    _button.gameObject.SetActive(false);
                    break;
                }
                case CardState.unlocked:
                {
                    _cardHider.SetActive(false);
                    _lock.SetActive(false);
                    _picture.SetActive(true);
                    _text.gameObject.SetActive(false);
                    _button.gameObject.SetActive(true);
                    break;
                }
                case CardState.current:
                {
                    _cardHider.SetActive(true);
                    _lock.SetActive(false);
                    _picture.SetActive(false);
                    _text.gameObject.SetActive(true);
                    _button.gameObject.SetActive(true);
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
        
        public void SetIndex(int index)
        {
            _index = index;
        }
    }
}