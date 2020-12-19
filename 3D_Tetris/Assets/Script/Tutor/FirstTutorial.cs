using System;
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
        [SerializeField] private CanvasGroup _sixthTutor;

        private int _amountSetElements = 0; 
        
        private Action OnMoveSuccess;
        private void Start()
        {
            RealizationBox.Instance.FSM.OnStart += StartGame;
        }

        void StartGame()
        {
            RealizationBox.Instance.tapsEvents.enabled = false;
            Invoke(nameof(FirstStep), _timeStop);
            
            _firstTutor.DOFade(0, 0.1f);
            _secondTutor.DOFade(0, 0.1f);
            _thirdTutor.DOFade(0, 0.1f);
            _fourthTutor.DOFade(0, 0.1f);
            _sixthTutor.DOFade(0, 0.1f);
            
            _amountSetElements = 0; 
        }
        
        void FirstStep() // open joystick
        {
            //pause
            RealizationBox.Instance.slowManager.SetPauseSlow(true);
            // text
            _firstTutor.DOFade(1, 0.3f);
            
            // tap event
            RealizationBox.Instance.tapsEvents.enabled = true;
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.OnlySingleTap;
            
            RealizationBox.Instance.tapsEvents.OnSingleTap += SecondStep;

        }

        void SecondStep() // move element
        {
            RealizationBox.Instance.tapsEvents.OnSingleTap -= SecondStep;
            
            _firstTutor.DOFade(0, 0.1f);
            _secondTutor.DOFade(1, 0.3f);
            
            RealizationBox.Instance.generator._answerElement.gameObject.SetActive(true);
            RealizationBox.Instance.gameController.onMoveApply += FinishMove;
            OnMoveSuccess += ThirdStep;
        }

        void ThirdStep() // double tap
        {
            OnMoveSuccess -= ThirdStep;
            
            RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.SingleAndDouble;
            
            _thirdTutor.DOFade(1, 0.1f);
            _secondTutor.DOFade(0, 0.3f);
            
            RealizationBox.Instance.tapsEvents.OnDoubleTap += FourthStep;
        }

        void FourthStep() // continue placing elements 
        {
            RealizationBox.Instance.tapsEvents.OnDoubleTap -= FourthStep;
            
            _fourthTutor.DOFade(1, 0.1f);
            _thirdTutor.DOFade(0, 0.3f);
            
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
            RealizationBox.Instance.tapsEvents.OnDoubleTap += FifthStep;
            RealizationBox.Instance.tapsEvents.OnSingleTap += FifthStep;
        }

        void FifthStep()
        {
            RealizationBox.Instance.tapsEvents.OnDoubleTap -= FifthStep;
            RealizationBox.Instance.tapsEvents.OnSingleTap -= FifthStep;

            _fourthTutor.DOFade(0, 0.3f);
            ElementData.onNewElementUpdate +=  SixthStep;
        }

        private void SixthStep() // drag the island to turn
        {
            if (++_amountSetElements > 2)
            {
                ElementData.onNewElementUpdate -=  SixthStep;
                
                RealizationBox.Instance.generator._answerElement.gameObject.SetActive(false);
                _sixthTutor.DOFade(1, 0.3f);
                RealizationBox.Instance.tapsEvents._blockTapEvents = BlockingType.None;
                RealizationBox.Instance.slowManager.SetPauseSlow(true);
                RealizationBox.Instance.tapsEvents.OnDragIceIsland += Finished;
            }
        }

        private void Finished()
        {
            RealizationBox.Instance.slowManager.SetPauseSlow(false);
            RealizationBox.Instance.tapsEvents.OnDragIceIsland -= Finished;
            _sixthTutor.DOFade(0, 0.3f);
        }

        

        void FinishMove(bool isSuccess, move direction)
        {
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