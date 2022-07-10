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
    }
    
    public class PlayerSaveProfile :MonoBehaviourSingleton<PlayerSaveProfile>
    {
        public Action<int> onLevelChange;
        public Action<int> onBestScoreChange;
        
        public int _lvl => _data.lvl;
        public int _bestScore => _data.bestScore;
        
        private SaveData _data;
        [SerializeField] private LvlList _lvlList;
        
        protected override void Awake()
        {
            base.Awake();
            Load();
            UpdateLvlData();
        }

        
        public void IncrementLvl()
        {
            _data.lvl++;
            onLevelChange?.Invoke(_data.lvl);
            Save();
        }
        
        public void SetCompletesLvl(int lvl)
        {
            _data.completedLvlData = lvl;
            onLevelChange?.Invoke(lvl);
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
                _data = new SaveData();
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
            _data = new SaveData();
        }

        public void UpdateLvlData()
        {
            if (_data.completedLvlData == _data.currentLvlData)
            {
                IncrementLvl();
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
    }
}