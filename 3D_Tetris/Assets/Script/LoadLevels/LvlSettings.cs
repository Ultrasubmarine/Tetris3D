using System;
using System.Collections.Generic;
using Script;
using Script.GameLogic;
using Script.GameLogic.Stars;
using Script.Speed; // for ChangeSpeedInfo
using UnityEngine;
using UnityEngine.Serialization;

namespace Script
{
    [Serializable]
    public struct StarSettings
    {
        public bool collectStar;
        public int maxStarsInPlace;
        public int winAmount;
        public int stepsBetweenStar;
        public int firstStep;
        public bool allPlaceInFirstStep;
        public bool onlyStarPlace;
    }
    
    [Serializable]
    public struct GeneratorSettings
    {
        public List<ChangeGeneratorInfo> points;
        public ProbabilitySettings probabilitySettings;
        public bool exceptCurrentElementForNext;
    }

    [Serializable]
    public struct BombsSettings
    {
        public bool makeBombs;
        public int bombStep;
        public int currentStep;
    }
    
    [Serializable]
    public struct NextBigBombOfferSettings
    {
        public int betweenOffersSteps;
        public int inOneGameMax;
    }

    [Serializable]
    public struct EvilBoxSettings
    {
        public bool lvlWithEvilBox;
        public int boxStep;
        public int currentBoxStep;
    }
        
    
    [CreateAssetMenu(fileName = "LvlSettings", menuName = "Lvl", order = 0)]
    public class LvlSettings : ScriptableObject
    {
        public int lvl => _lvl;
        public bool infinity => _infinity;
        public TutorType tutorType { get {return _tutorType; } }
        public List<ChangeSpeedInfo> speedSettings => _speedSettings;
        public int winScore => _winScore;
        public StarSettings starSettings => _starSettings;
        public GeneratorSettings generatorSettings => _generatorSettings;
        public List<CreatedElement> lvlElements => _lvlElements;
        public TetrisState startState => _startState;
        public List<StarPlace> starPlaces => _starPlaces;
        public BombsSettings bombsSettings => _bombsSettings;
        public NextBigBombOfferSettings nextBombOfferSettings => _nextBigBombOfferSettings;
        public EvilBoxSettings evilBoxSettings => _evilBoxSettings;
        
        [SerializeField] private TutorType _tutorType;
        [SerializeField] private List<ChangeSpeedInfo> _speedSettings;
        [SerializeField] private int _winScore;
        [SerializeField] private StarSettings _starSettings;
        [SerializeField] private GeneratorSettings _generatorSettings;
        [SerializeField] private List<CreatedElement> _lvlElements;
        [SerializeField] private TetrisState _startState = TetrisState.GenerateElement;
        [SerializeField] private List<StarPlace> _starPlaces;
        [SerializeField] private BombsSettings _bombsSettings;
        [SerializeField] private NextBigBombOfferSettings _nextBigBombOfferSettings;
        [SerializeField] private EvilBoxSettings _evilBoxSettings;
        
        [SerializeField] private int _lvl;
        [SerializeField] private bool _infinity;
    }
}