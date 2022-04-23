using System;
using System.Collections.Generic;
using DG.Tweening;
using Script;
using Script.GameLogic;
using Script.GameLogic.Stars;
using Script.GameLogic.TetrisElement;
using Script.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _losePanels;
    
    [SerializeField] private BottomPanelAnimation _settingsPanel;
    [SerializeField] private BottomPanelAnimation _gameplayBottomPanel;
    
    [SerializeField] private CanvasGroup _winPanel;
    
    private TetrisFSM _fsm;

    private TetrisState _startState;
    
    public event Action OnReplay;

    private void Start()
    {
        _fsm = RealizationBox.Instance.FSM;
        LoadLvlSettings();
        RealizationBox.Instance.lvlElementsSetter.Init();
        RealizationBox.Instance.lvlElementsSetter.CreateElements();
        Invoke( nameof(LastStart), 1f);

        DOTween.defaultAutoKill = true;
    }

    private void LoadLvlSettings()
    {
        var lvl = LvlLoader.instance.lvlSettings;
        var box = RealizationBox.Instance;

        if (lvl.tutorType != TutorType.None)
        {
            GameObject tutor = RealizationBox.Instance.GetTutor(lvl.tutorType);
            tutor.SetActive(true);
        }


        box.speedChanger.SetSpeedPoints(lvl.speedSettings);
        
        box.score.SetWinScore(lvl.winScore);

        box.starsManager.neededStars = lvl.starSettings.winAmount;
        box.starsManager.collectStarLvlLvl = lvl.starSettings.collectStar;
        box.starsManager.maxStarsAmount = lvl.starSettings.maxStarsInPlace;
        box.starsManager.stepsBetweenStar = lvl.starSettings.stepsBetweenStar;
        box.starsManager.starPlaces = new List<StarPlace>(lvl.starPlaces);
        
        box.generatorChanger.SetGeneratorSettings(lvl.generatorSettings.points);
        box.generator._probabilitySettings = lvl.generatorSettings.probabilitySettings;

        box.lvlElementsSetter.createdElements = new List<CreatedElement>(lvl.lvlElements);
        box.FSM.startState= lvl.startState;
        _startState = lvl.startState;  
    }

    private void LastStart()
    {
        _fsm.AddListener(TetrisState.LoseGame, OnLoseGame);
    }

    public void StartGame()
    {
        _fsm.StartFSMFromCustomState(_startState);
    //    _fsm.StartFSM();
    }

    private void OnLoseGame()
    {
        foreach (var panel in _losePanels)
        {
            panel.SetActive(true);
        }
        _settingsPanel.Show();
        _gameplayBottomPanel.Hide();
    }
    
    private void OnWinGame()
    {
        _winPanel.gameObject.SetActive(true);
    }

    public void Restart()
    {
        _fsm.SetNewState(TetrisState.Restart);
    }
    
    public void ClearPlace()
    {
        RealizationBox.Instance.starsManager.Clear();  
        
        RealizationBox.Instance.projectionLineManager.Clear();
        RealizationBox.Instance.projection.Destroy();
        
        RealizationBox.Instance.elementCleaner.DeleteAllElements();
      //  RealizationBox.Instance.generator.Clear();
        RealizationBox.Instance.influenceManager.ClearAllInfluences();
        RealizationBox.Instance.matrix.Clear();
        
        RealizationBox.Instance.slowManager.DeleteAllSlows();
        RealizationBox.Instance.lvlElementsSetter.CreateElements();  
        OnReplay?.Invoke();
        RealizationBox.Instance.haightHandler.CalculateHeight();
        RealizationBox.Instance.gameCamera.SetPositionWithoutAnimation();
        RealizationBox.Instance.speedChanger.ResetSpeed();
        RealizationBox.Instance.generatorChanger.ResetGenerator();
       
        
        //Add boosters
    }

    public void SetReplayState()
    {
        _fsm.SetNewState(TetrisState.Restart);
    }
    
    public void Preconstruct()
    {
        var lvl = LvlLoader.instance.lvlSettings;
        var box = RealizationBox.Instance;
        
       
        box.FSM.startState= lvl.startState;
        box.starsManager.starPlaces = new List<StarPlace>(lvl.starPlaces);
    }
    public void ShowWinPanel()
    {
        _winPanel.gameObject.SetActive(true);
        _winPanel.DOFade(1, 0.4f);
        _winPanel.transform.DOMoveY(_winPanel.transform.position.y, 0.4f).From(_winPanel.transform.position.y - 250);
    }
}