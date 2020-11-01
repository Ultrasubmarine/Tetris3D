using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Script.Controller.TouchController
{
    
    public class IslandTurn : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] private GameObject island;
        
        private float firstPosition;

        private float lastPosition;

        private float delta = 0;// 2;

        private bool isTurn = false;
        
        public void Turn(bool state)
        {
            isTurn = state;
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
        }
    }
}