using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Controller.TouchController
{
    
    public class IslandTurn : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] private GameObject island;

        [SerializeField] private float _speedForCorrectRotate = 0.5f;

        private float firstPosition;

        private float lastPosition;

        private float delta = 0;// 2;

        private bool isTurn = false;

        private GameController _gameController;

        public event Action OnStartTurn;

        public event Action OnEndTurn;
        
        private void Start()
        {
            _gameController = RealizationBox.Instance.gameController;
            RealizationBox.Instance.tapsEvents.OnDragIceIsland += () =>Turn(true);
        }

        public void Turn(bool state)
        {
            island.transform.DOKill();
            isTurn = state;
            lastPosition = Input.mousePosition.x;
            OnStartTurn.Invoke();
        }

        private void Update()
        {
            if (!isTurn)
                return;

            if (Mathf.Abs(Input.mousePosition.x - lastPosition) > delta)
            {
                float angle = 180 * (lastPosition - Input.mousePosition.x ) / (Screen.width * 0.8f);
    
                island.transform.Rotate(Vector3.up, angle );
                lastPosition = Input.mousePosition.x;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Turn(false);
            TurnFinished();
            OnEndTurn.Invoke();
        }

        private void TurnFinished()
        {
            var sinY = Mathf.Sin(island.transform.eulerAngles.y * Mathf.Deg2Rad);
            
            float needRotate;

            if (sinY >= 0.5f)
                needRotate = 90;
            else if (sinY <= -0.5f)
                needRotate = 270;
            else if (island.transform.eulerAngles.y > 90 && island.transform.eulerAngles.y < 270)
                needRotate = 180; 
            else
                needRotate = 0;
            
            island.transform.DORotate(new Vector3(0, needRotate, 0), _speedForCorrectRotate );
            _gameController.CorrectTurn((int)needRotate);
        }

        private void SwipeTurn()
        {
            
        }
    }
}