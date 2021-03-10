using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.Tutor
{
    public class FirstTutorial : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _firstTutor;
        [SerializeField] private float _timeStop;

        [SerializeField] private CanvasGroup _secondTutor;
        [SerializeField] private CanvasGroup _thirdTutor;
        [SerializeField] private CanvasGroup _fourthTutor;

        [SerializeField] private CanvasGroup _topPanel;
        [SerializeField] private CanvasGroup _bottomPanel;
        [SerializeField] private RectTransform _hand;
        private int _amountSetElements = 0; 
        
        private Action OnMoveSuccess;
        private void Start()
        {
            RealizationBox.Instance.FSM.OnStart += StartGame;
            _topPanel.alpha = 0;
            _bottomPanel.alpha = 0;
            
            _topPanel.gameObject.SetActive(false);
            _bottomPanel.gameObject.SetActive(false);
            
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.OnlySingleTap;
            RealizationBox.Instance.generator.fixedHightPosition = 10;
        }

        void StartGame()
        {
          //  global::Speed.SetTimeDrop(0.26f);
            RealizationBox.Instance.tapsEvents.enabled = false;
           // Invoke(nameof(FirstStep), _timeStop);
            
            _firstTutor.DOFade(0, 0.1f);
            _secondTutor.DOFade(0, 0.1f);
            _thirdTutor.DOFade(0, 0.1f);
            _fourthTutor.DOFade(0, 0.1f);
            
            _amountSetElements = 0;
            _topPanel.alpha = 0;
            _topPanel.interactable = false;
            _bottomPanel.alpha = 0;
            _bottomPanel.interactable = false;

            FirstStep();
        }
        
        void FirstStep() // open joystick
        {   
            Invoke(nameof(FirstStepPause),_timeStop);
            // text
            _firstTutor.DOFade(1, 1f);

            // tap event
            RealizationBox.Instance.tapsEvents.enabled = true;
            RealizationBox.Instance.tapsEvents.OnSingleTap += SecondStep;
        }

        void FirstStepPause()
        {
            //pause
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
        }

        void SecondStep() // move element
        {
            RealizationBox.Instance.speedChanger.ResetSpeed();
            RealizationBox.Instance.tapsEvents.OnSingleTap -= SecondStep;
            
            _firstTutor.DOFade(0, 0.1f).SetDelay(0.1f).OnComplete(() => _secondTutor.DOFade(1, 0.2f));
            
            IEnumerable<CoordinatXZ> blocksXZ, blocksAnswerXZ, razn;
            do
            {
                blocksXZ = ElementData.newElement.blocks.Select(b => b.xz);
                blocksAnswerXZ = RealizationBox.Instance.generator._answerElement.blocks.Select(b => b.xz);
                razn = blocksXZ.Except(blocksAnswerXZ);
                
                RealizationBox.Instance.generator.SetRandomPosition(RealizationBox.Instance.generator._answerElement);
            } while (!razn.Any());
            
            RealizationBox.Instance.generator._answerElement.gameObject.SetActive(true);
            RealizationBox.Instance.FSM.onStateChange += FinishMove;
            OnMoveSuccess += ThirdStep;
        }

        void ThirdStep() // double tap
        {
            OnMoveSuccess -= ThirdStep;
            
            _secondTutor.DOKill();
            _secondTutor.DOFade(0, 0.3f).SetDelay(0.2f).
                OnComplete(() => _thirdTutor.DOFade(1, 0.1f).OnComplete(() =>
                {
                    RealizationBox.Instance.tapsEvents.OnDoubleTap += FourthStep;
                    RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDouble;
                }));
        }

        void FourthStep() // continue placing elements 
        {
            RealizationBox.Instance.tapsEvents.OnDoubleTap -= FourthStep;
            _thirdTutor.DOKill();
            _thirdTutor.DOFade(0, 0.3f);
            
            ElementData.onNewElementUpdate +=  SixthStep;
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
        }


        private void SixthStep() // drag the island to turn
        {
            if (++_amountSetElements > 1)
            {
                ElementData.onNewElementUpdate -=  SixthStep;
                
                RealizationBox.Instance.tapsEvents.enabled = false;
                RealizationBox.Instance.generator._answerElement.gameObject.SetActive(false);
                Invoke(nameof(SeventhStep), 2.2f);
            }
        }

        private void SeventhStep()
        {
            RealizationBox.Instance.tapsEvents.enabled = true;
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
            
            _fourthTutor.DOFade(1, 0.3f);
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDrag;
            RealizationBox.Instance.tapsEvents.OnDragIceIsland += Finished;

            int w = Screen.width / 2;
            _hand.DOMoveX(w - w/3, 1.0f).From(w + w/3).SetLoops(-1, LoopType.Yoyo);
        }
        
        private void Finished()
        {
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
            RealizationBox.Instance.tapsEvents.OnDragIceIsland -= Finished;
            _fourthTutor.DOFade(0, 0.3f).OnComplete(() =>
            {
                _topPanel.DOFade(1, 0.6f);
                _bottomPanel.DOFade(1, 0.6f);
            });
            
            _topPanel.interactable = true;
            _bottomPanel.interactable = true;
            RealizationBox.Instance.FSM.OnStart -= StartGame;
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.None;
            _hand.DOKill();
            
            RealizationBox.Instance.FSM.onStateChange -= FinishMove;
        }

        

        void FinishMove(TetrisState obj)
        {
            if (obj != TetrisState.EndInfluence)
                return;
            
            var blocksXZ = ElementData.newElement.blocks.Select(b => b.xz);
            
            var blocksAnswerXZ = RealizationBox.Instance.generator._answerElement.blocks.Select(b => b.xz);

            var razn = blocksXZ.Except(blocksAnswerXZ);
            if (razn == null || razn.Count() == 0)
            {
                OnMoveSuccess?.Invoke();
            }
        }
 
    }
}