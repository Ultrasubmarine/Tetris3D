using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace Script.PlayerProfile
{
    [Serializable]
    class SaveData
    {
        public int lvl = 0;
        public int bombAmount = 0 ;
        public int bestScore = 0;
    }
    
    public class PlayerSaveProfile :MonoBehaviourSingleton<PlayerSaveProfile>
    {
        public Action<int> onLevelChange;
        public Action<int> onBombAmountChange;
        public Action<int> onBestScoreChange;
        
        public int _lvl => _data.lvl;
        public int _bombAmount => _data.bombAmount;
        public int _bestScore => _data.bestScore;
        
        private SaveData _data;


        protected override void Awake()
        {
            base.Awake();
            Load();
        }

        public void SetLvl(int lvl)
        {
            _data.lvl = lvl;
            onLevelChange?.Invoke(lvl);
            Save();
        }
        
        public void SetBombAmount(int amount)
        {
            _data.bombAmount = amount;
            onBombAmountChange?.Invoke(amount);
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
    }
}