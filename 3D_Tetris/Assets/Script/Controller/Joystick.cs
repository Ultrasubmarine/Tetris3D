using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Controller
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Joystick : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler
    {
     //   [SerializeField] private Sprite _base;
        
        [SerializeField] private RectTransform _stick;
        
        [SerializeField] private Canvas _canvas;
        
        private Vector2 _deltaSize;
        
        private RectTransform _transform;
        
        private CanvasGroup _canvasGroup;

        private MoveTouchController _moveTouchController;
        
        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _moveTouchController = RealizationBox.Instance.moveTouchController;
            _moveTouchController.onStateChanged += OnMoveTouchControllerStateChange;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            
            _transform.position/*var center*/ = Input.GetTouch(0).position;

            /*_transform.position = new Vector2(center.x  * _deltaSize.x / Screen.width, 
                (center.y  * _deltaSize.y / Screen.height));*/
          
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Drag");
            _stick.position = Input.GetTouch(0).position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
        
        private void OnMoveTouchControllerStateChange( MoveTouchController.StateTouch stateTouch)
        {
            switch (stateTouch)
            {
                case MoveTouchController.StateTouch.open:
                {
                    Show();
                    break;
                }
                case MoveTouchController.StateTouch.none:
                {
                    Hide();
                    break;
                }
            }
        }
    }
}