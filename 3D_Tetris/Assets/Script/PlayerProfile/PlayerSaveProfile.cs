using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.PlayerProfile
{
    
    [Serializable]
    class SaveData
    {
        public int currentLvlData = 0;
        public int completedLvlData = -1;
        
        public int lvl = 0;
        public int bestScore = 0;

        public Dictionary<Currency, int> wallet;
        public bool x2Reward = false;

        public int currentCard = 0;
        public List<int> openedCardParts;
        
        public SaveData(Dictionary<Currency, int> wallet, List<int> openedCardParts)
        {
            this.wallet = wallet;
            this.openedCardParts = openedCardParts;
        }
    }

    [Serializable]
    public enum Currency
    {
        stars,
        coin,
    }
    
    public class PlayerSaveProfile :MonoBehaviourSingleton<PlayerSaveProfile>
    {
        public Action<int> onLevelChange;
        public Action<int> onBestScoreChange;
        
        public Action<int> onCurrentCardChange;
        public Action<int> onOpenCardPartsAmpuntChange;

        public Action<Currency, int> onCurrencyAmountChanged;
            
        public int _lvl => _data.lvl;
        public int _bestScore => _data.bestScore;

        public int _currentCardIndex => _data.currentCard;
        public List<int> _openedCardParts => _data.openedCardParts;
        
        private SaveData _data;
        [SerializeField] private LvlList _lvlList;

        public int GetCurrencyAmount(Currency type)
        {
            if (!_data.wallet.ContainsKey(type))
                return 0;
            return _data.wallet[type];
        }
        
        protected override void Awake()
        {
            base.Awake();
            Load();
            CheckWin();
        }

        public void Add5Stars()
        {
            ChangeCurrencyAmount(Currency.coin, 5);
        }
        public bool ChangeCurrencyAmount(Currency type, int offset)
        {
            if (_data.wallet.ContainsKey(type))
            {
                if(_data.wallet[type] + offset < 0)
                {
                    return false;
                }
                int current = _data.wallet[type];
                _data.wallet[type] = current + offset;
            }
            else
            {
                if (offset < 0)
                {
                    return false;
                }
                _data.wallet[type] = offset;
            }
            
            onCurrencyAmountChanged?.Invoke(type,_data.wallet[type]);
            Save();
            return true;
        }
        
        public void IncrementLvl()
        {
            _data.lvl++;
            onLevelChange?.Invoke(_data.lvl);
            Save();
        }
        
        public void CompleteCurrentLvl()
        {
            _data.completedLvlData = _data.currentLvlData;
            onLevelChange?.Invoke( _data.completedLvlData);
            Save();
        }
        
        public void SetBestScore(int score)
        {
            _data.bestScore = score;
            onBestScoreChange?.Invoke(score);
            Save();
        }
        
        public void Load()
        {
           // _lvl = Unity.
           
            if (!File.Exists(Application.persistentDataPath + "/MySaveData.tds"))
            {
                _data = new SaveData(new Dictionary<Currency, int>(), new List<int>());
            }
            else
            { 
                FileStream file = File.Open(Application.persistentDataPath 
                                            + "/MySaveData.tds", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter(); 
                _data =(SaveData)bf.Deserialize(file);
                file.Close();
            }
        }

        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter(); 
            FileStream file = File.Create(Application.persistentDataPath 
                                          + "/MySaveData.tds");
            bf.Serialize(file, _data);
            file.Close();
            Debug.Log("Game data saved!");
            
        }

        public void ResetSave()
        {
            if (File.Exists(Application.persistentDataPath + "/MySaveData.tds"))
            {
                File.Delete(Application.persistentDataPath + "/MySaveData.tds");
            }
            _data = new SaveData(new Dictionary<Currency, int>(),new List<int>());
        }

        public void CheckWin()
        {
            if (_data.completedLvlData == _data.currentLvlData)
            {
                AddReward(GetCurrentLvlData(),_data.x2Reward);
                
                _data.x2Reward = false;
                _data.lvl++;
                if (_data.lvl <= _lvlList.lvls.Length - 1)
                    _data.currentLvlData = _data.lvl;
                else
                {
                    do
                    {
                        _data.currentLvlData = Random.Range(_lvlList.firstRepeatLvl, _lvlList.lvls.Length);
                    } while (_data.currentLvlData == _data.completedLvlData );
                }
                
                Save();
            }
        }

        private void AddReward(LvlSettings completedLvl, bool x2 = false)
        {
            int reward = completedLvl.starSettings.winAmount;
            
            if (x2)
                reward *= 2;

            ChangeCurrencyAmount(Currency.stars, reward);
            ChangeCurrencyAmount(Currency.coin, reward*2 );
        }
        public LvlSettings GetCurrentLvlData()
        {
            return _lvlList.lvls[_data.currentLvlData];
        }

        public void SetRewardX2()
        {
            _data.x2Reward = true;
            Save();
        }

        public void AddUnlockCardPart(int partIndex)
        {
            _data.openedCardParts.Add(partIndex);
            onOpenCardPartsAmpuntChange?.Invoke(_data.openedCardParts.Count);
            Save();
        }

        public void ResetUnlockedCardParts()
        {
            _data.openedCardParts.Clear();
            onOpenCardPartsAmpuntChange?.Invoke(_data.openedCardParts.Count);
            Save();
        }
        
        public void IncrementCurrentCard()
        {
            _data.currentCard++;
            onCurrentCardChange?.Invoke(_data.currentCard);
            Save();
        }
    }
}