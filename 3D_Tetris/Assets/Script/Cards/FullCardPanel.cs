using UnityEngine;
using UnityEngine.UI;

namespace Script.Cards
{
    public class FullCardPanel : MonoBehaviour
    {
        public CanvasGroup canvasGroup { get; private set; }
        public Button closeBtn => _closeBtn;
        [SerializeField] private Button _closeBtn;
        
        [SerializeField] private Image _image;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void SetImage( Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}