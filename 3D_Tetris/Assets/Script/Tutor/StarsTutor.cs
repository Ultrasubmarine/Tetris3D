using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Script.Controller;
using Script.GameLogic.TetrisElement;

namespace Script.Tutor
{
    public class StarsTutor: MonoBehaviour
    {
        [SerializeField] private float _timeStop = 1.15f;
        
        [SerializeField] private CanvasGroup _tutor;
        [SerializeField] private CanvasGroup _topPanel;
        [SerializeField] private CanvasGroup _bottomPanel;

        private bool _generateNeedElement;

        private bool _canPlace = false;
        
        private void Start()
        {
            _topPanel.alpha = 0;
            _bottomPanel.alpha = 0;
            
            _topPanel.gameObject.SetActive(false);
            _bottomPanel.gameObject.SetActive(false);

            RealizationBox.Instance.tapsEvents.enabled = false;
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.OnlySingleTap;
            
            RealizationBox.Instance.FSM.onStateChange+= FirstStep;
            RealizationBox.Instance.FSM.onStateChange += OnGenerateFirstElement;
            
            RealizationBox.Instance.generator.fixedHightPosition = 10;
            _generateNeedElement = RealizationBox.Instance.generator._generateNeedElement;
            RealizationBox.Instance.generator._generateNeedElement = true;
        }

        void OnGenerateFirstElement(TetrisState state )
        {
            if (state != TetrisState.GenerateElement)
                return;
            RealizationBox.Instance.FSM.onStateChange -= OnGenerateFirstElement;
            
            IEnumerable<CoordinatXZ> razn;
            do
            {
                IEnumerable<CoordinatXZ> blocksXZ, blocksAnswerXZ;
                blocksXZ = ElementData.newElement.blocks.Select(b => b.xz);
                blocksAnswerXZ = RealizationBox.Instance.generator._answerElement.blocks.Select(b => b.xz);
                razn = blocksXZ.Except(blocksAnswerXZ);

                if (!razn.Any())
                {
                    RealizationBox.Instance.generator.SetRandomPosition(ElementData.newElement);
                    RealizationBox.Instance.projectionLineManager.UpdateProjectionLines();
                    RealizationBox.Instance.projection.CreateProjection();
                }
                   
            } while (!razn.Any());
        }
      
        void FirstStep(TetrisState state)
        {
            // if (state == TetrisState.CreateStar)
            // {
            //     _tutor.DOFade(1, 1f);
            //     return;
            // }
             if (state == TetrisState.GenerateElement)
            {
                RealizationBox.Instance.FSM.onStateChange -= FirstStep;
                RealizationBox.Instance.tapsEvents.enabled = true;
            }
            else
            {
                return;
            }
            Invoke(nameof(FirstStepPause),_timeStop);

            _tutor.DOFade(1, 1f);
            RealizationBox.Instance.generator._answerElement.gameObject.SetActive(true);
            
            IEnumerable<CoordinatXZ> blocksXZ, blocksAnswerXZ, razn;
            blocksXZ = ElementData.newElement.blocks.Select(b => b.xz);
            blocksAnswerXZ = RealizationBox.Instance.generator._answerElement.blocks.Select(b => b.xz);
            razn = blocksXZ.Except(blocksAnswerXZ);
            if (!razn.Any())
            {
                _canPlace = true;
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDouble;
            }

            RealizationBox.Instance.joystick.onStateChange += FinishMove;
            RealizationBox.Instance.tapsEvents.OnDoubleTap += Finish;
        }
        
        void FirstStepPause()
        {
            //pause
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
        }
        
        void FinishMove(JoystickState state)
        {
            IEnumerable<CoordinatXZ> blocksXZ, blocksAnswerXZ, razn;
            blocksXZ = ElementData.newElement.blocks.Select(b => b.xz);
            blocksAnswerXZ = RealizationBox.Instance.generator._answerElement.blocks.Select(b => b.xz);
            razn = blocksXZ.Except(blocksAnswerXZ);
            if (!razn.Any())
            {
                _canPlace = true;
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDouble;
            }
            else
            {
                _canPlace = true;
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.OnlySingleTap;
            }
        }
        
        void Finish()
        {
            RealizationBox.Instance.joystick.onStateChange -= FinishMove;
            RealizationBox.Instance.tapsEvents.OnDoubleTap -= Finish;
            RealizationBox.Instance.FSM.onStateChange += HideAnswerElement;
            
            _tutor.DOFade(0, 1f).OnComplete(() =>
            {
                RealizationBox.Instance.slowManager.SetPauseSlow(false);
                RealizationBox.Instance.tapsEvents.enabled = true;
                RealizationBox.Instance.generator._generateNeedElement = _generateNeedElement;
                gameObject.SetActive(false);
            });
            
           _topPanel.DOFade(1, 0.6f);
           _bottomPanel.DOFade(1, 0.6f);
           
           RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.None;
        }

        void HideAnswerElement(TetrisState state)
        {
            if (state == TetrisState.MergeElement)
            {
                RealizationBox.Instance.generator._answerElement.gameObject.SetActive(false);
                RealizationBox.Instance.FSM.onStateChange -= HideAnswerElement;
            }
        }
    }
}