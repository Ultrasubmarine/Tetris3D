using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Booster
{
    public enum Booster
    {
        Freeze,
        AnswerElement,
    }

    public class BoostersManager : MonoBehaviour
    {
        [SerializeField] private List<BoosterBase> _boosters;

        [SerializeField] private GameObject _boosterUi;

        [SerializeField] private RectTransform _boosterUiParent;

        private List<BoosterUi> _boosterUis;

        private void Start()
        {
            RealizationBox.Instance.gameManager.OnReplay += ResetBoosters;
            
            _boosterUis = new List<BoosterUi>();
            foreach (var booster in _boosters)
            {
                booster.Initialize();

                var ui = Instantiate(_boosterUi).GetComponent<BoosterUi>();
                ui.Initialize(booster);
                ui.transform.parent = _boosterUiParent;
         
                _boosterUis.Add(ui);
            }
        }

        public void ResetBoosters()
        {
            foreach (var booster in _boosters)
            {
                booster.Reset();
            }
        }
    }
}