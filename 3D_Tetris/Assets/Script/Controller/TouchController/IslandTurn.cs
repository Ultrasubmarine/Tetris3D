using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.EventSystems;

namespace Script.Controller.TouchController
{
    
    public class IslandTurn : MonoBehaviour, IPointerExitHandler, IPointerUpHandler
    {
        public bool isTurnIsland => isTurn;
        
        [SerializeField] private GameObject island;
        [SerializeField] public List<Transform> extraTurn;
        
        [SerializeField] private float _speedForCorrectRotate = 0.5f;
        [SerializeField] private float _speedForCorrectRotateForSwipe = 0.5f;
        
        private float firstPosition;
        private float lastPosition;
        
        private int _firstAngle;

        private float delta = 0;// 2;

        private bool isTurn = false;

        private GameController _gameController;

        public event Action OnStartTurn;

        public event Action OnEndTurn;

        private float _startTime;

        private const float _swipeTime = 0.3f;
        
        private void Start()
        {
            _gameController = RealizationBox.Instance.gameController;
            RealizationBox.Instance.tapsEvents.OnDragIceIsland += () =>Turn(true);

            RealizationBox.Instance.FSM.OnStart += ResetTurn;
        }
        
        public void Turn(bool state)
        {
          //  if (state && RealizationBox.Instance.FSM.GetCurrentState() == TetrisState.CreateStar)
          //      return;
            island.transform.DOComplete();
            foreach (var item in extraTurn) item.DOComplete();

            isTurn = state;
            
            if (state)
            {
                firstPosition = Input.mousePosition.x;

                _firstAngle = (int)island.transform.eulerAngles.y;
                _startTime = Time.time;
                OnStartTurn?.Invoke();
            }
            lastPosition = Input.mousePosition.x;
        }

        private void Update()
        {
            if (!isTurn)
                return;

            if (Mathf.Abs(Input.mousePosition.x - lastPosition) > delta)
            {
                float angle = 180 * (lastPosition - Input.mousePosition.x ) / (Screen.width * 0.8f);

                angle = Mathf.Clamp(angle, -90.0f / _swipeTime, 90.0f / _swipeTime);
                island.transform.Rotate(Vector3.up, angle );
                foreach (var item in extraTurn) item.Rotate(Vector3.up, angle );
                lastPosition = Input.mousePosition.x;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Turn(false);
            TurnFinished();
        }

        private void TurnFinished()
        {
            int needRotate;
            float speed;
            if (Time.time - _startTime < _swipeTime)  // if short swipe
            {
                if (firstPosition - lastPosition > 0) //left
                    needRotate = _firstAngle + 90;
                else
                    needRotate = _firstAngle - 90;

                speed = _speedForCorrectRotateForSwipe;
            }
            else // if drag island
            {
                var sinY = Mathf.Sin(island.transform.eulerAngles.y * Mathf.Deg2Rad);

                if (sinY >= 0.5f)
                    needRotate = 90;
                else if (sinY <= -0.5f)
                    needRotate = 270;
                else if (island.transform.eulerAngles.y > 90 && island.transform.eulerAngles.y < 270)
                    needRotate = 180; 
                else
                    needRotate = 0;

                speed = _speedForCorrectRotate;
            }
            
            island.transform.DORotate(new Vector3(0, needRotate, 0), speed).OnComplete(() => OnEndTurn.Invoke());
            foreach (var item in extraTurn) item.DORotate(new Vector3(0, needRotate, 0), speed);
            _gameController.CorrectTurn((int)needRotate);
        }

        public void ResetTurn()
        {
            island.transform.DORotate(new Vector3(0, 0, 0), 0);
            foreach (var item in extraTurn) item.DORotate(new Vector3(0, 0, 0), 0);
            _gameController.CorrectTurn(0);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnEndTurn.Invoke();
        }
    }
}