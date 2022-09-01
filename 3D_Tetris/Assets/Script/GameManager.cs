using System;
using System.Collections.Generic;
using DG.Tweening;
using Script;
using Script.GameLogic;
using Script.GameLogic.Stars;
using Script.GameLogic.TetrisElement;
using Script.Home;
using Script.PlayerProfile;
using Script.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int currentLvl { get; private set; }
    
    public bool infinity { get; private set; }
    
    [SerializeField] private GameObject[] _losePanels;
    
    [SerializeField] private BottomPanelAnimation _settingsPanel;
    [SerializeField] private BottomPanelAnimation _gameplayBottomPanel;
    
    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private RectTransform[] _rewardOreols;
    [SerializeField] private CurrencyLabel _starReward, _coinReward;
    [SerializeField] private GameObject _rewardX2Btn;
    
    [SerializeField] private CanvasGroup _topGamePanel;
    private TetrisFSM _fsm;

    private TetrisState _startState;
    [SerializeField] private int multiplier = 5;
    [SerializeField] private TextMeshProUGUI _textMultiplier;
    
    
    [SerializeField] private CanvasGroup _continueBtn;
    [SerializeField] private float delayForContinueBtn;
    
    public event Action OnReplay;

    private void Awake()
    {
        infinity = LvlLoader.instance.lvlSettings.infinity;
    }

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
        
        currentLvl = lvl.lvl;
        infinity = lvl.infinity;
        
        if (lvl.tutorType != TutorType.None)
        {
            GameObject tutor = RealizationBox.Instance.GetTutor(lvl.tutorType);
            tutor.SetActive(true);
        }

        box.speedChanger.SetSpeedPoints(lvl.speedSettings);
        
        box.score.SetWinScore(lvl.winScore);

        box.starsManager.allPlaceInFirstStep = lvl.starSettings.allPlaceInFirstStep;
        box.starsManager.neededStars = lvl.starSettings.winAmount;
        box.starsManager.collectStarLvlLvl = lvl.starSettings.collectStar;
        box.starsManager.maxStarsAmount = lvl.starSettings.maxStarsInPlace;
        box.starsManager.stepsBetweenStar = lvl.starSettings.stepsBetweenStar;
        box.starsManager.starPlaces = new List<StarPlace>(lvl.starPlaces);
        box.starsManager.currentStep = lvl.starSettings.firstStep;
        box.starsManager.onlyStarPlace = lvl.starSettings.onlyStarPlace;
        
        box.generatorChanger.SetGeneratorSettings(lvl.generatorSettings.points);
        box.generator._probabilitySettings = lvl.generatorSettings.probabilitySettings;
        box.generator.exceptCurrentElementForNext = lvl.generatorSettings.exceptCurrentElementForNext;
        
        box.lvlElementsSetter.createdElements = new List<CreatedElement>(lvl.lvlElements);
        box.FSM.startState= lvl.startState;

        box.bombsManager.lvlWithBombs = lvl.bombsSettings.makeBombs;
        box.bombsManager._stepForBomb = lvl.bombsSettings.bombStep;
        box.bombsManager._currentStep = box.bombsManager._currentStepSave = lvl.bombsSettings.currentStep;
        
        box.evilBoxManager.lvlWithEvilBox = lvl.evilBoxSettings.lvlWithEvilBox;
        box.evilBoxManager._stepForBomb = lvl.evilBoxSettings.boxStep;
        box.evilBoxManager._currentStep = box.evilBoxManager._currentStepSave = lvl.evilBoxSettings.currentBoxStep;
      
        box.stoneBlockManager.lvlWithStone = lvl.stoneBlockSettings.lvlWithStoneBlocks;
        box.stoneBlockManager._stepForStoneBlock = lvl.stoneBlockSettings.stoneStep;
        box.stoneBlockManager._currentStep = box.stoneBlockManager._currentStepSave =  lvl.stoneBlockSettings.currentStoneStep;

        _startState = lvl.startState;
    }

    private void LastStart()
    {
      //  _fsm.AddListener(TetrisState.LoseGame, OnLoseGame);
    }

    public void StartGame()
    {
        _fsm.StartFSMFromCustomState(_startState);
    //    _fsm.StartFSM();
    }

    public void OnLoseGame()
    {
        foreach (var panel in _losePanels)
        {
            panel.SetActive(true);
        }
       // _settingsPanel.Show();
    //    _gameplayBottomPanel.Hide();
    }
    
    private void OnWinGame()
    {
        _winPanel.gameObject.SetActive(true);
        _textMultiplier.text = "x" + multiplier.ToString();
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

        _continueBtn.alpha = 0;
        _continueBtn.interactable = false;
        
        foreach (var o in _rewardOreols)
        {
            o.DORotate(new Vector3 (0, 0,360), 3f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);
        }

        var reward = LvlLoader.instance.lvlSettings.starSettings.winAmount;
        _starReward.SetCurrencyAmount( reward);
        _coinReward.SetCurrencyAmount( reward * 2);
        
        Invoke(nameof(ShowContinueBtn), delayForContinueBtn);
    }

    private void ShowContinueBtn()
    {
        _continueBtn.DOFade(1, 0.3f).From(0).OnComplete( () => _continueBtn.interactable = true);
    }
    public void ResetPause()
    {
        
    }

    public void SetRewardMultiply()
    {
        AdsManager.instance.ShowRewarded(b =>
        {
            if (b)
            {
                PlayerSaveProfile.instance.SetRewardMultiplier(multiplier);
        
                var reward = LvlLoader.instance.lvlSettings.starSettings.winAmount * multiplier;
                _starReward.OnCurrencyAmountChanged(Currency.stars,reward);
                _coinReward.OnCurrencyAmountChanged(Currency.coin,reward*2);
                _rewardX2Btn.SetActive(false);
        
                CancelInvoke(nameof(ShowContinueBtn));
                if(_continueBtn.alpha < 1)
                    ShowContinueBtn();
            }
        });
    }
    
    public void HideGamePanels()
    {
        _topGamePanel.DOFade(0, 0.1f);
        _topGamePanel.interactable = false;
    }

    public void SkipLvl()
    {
        AdsManager.instance.ShowRewarded(b =>
        {
            if (b)
            {
                PlayerSaveProfile.instance.IncrementLvl();
                LvlLoader.instance.Select(PlayerSaveProfile.instance.GetCurrentLvlData());
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        });
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && !AdsManager.instance.isShowingAds)
        {
            DOTween.KillAll();
            LvlLoader.instance.Select(PlayerSaveProfile.instance.GetCurrentLvlData());
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}