using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;

namespace ClickUpAdventurers
{
    #region SerializableClasses 

    [System.Serializable]
    public class PlayerEquipment
    {
        [SerializeField]
        public List<int> playerEquip;
    }

    [System.Serializable]
    public class PlayerBoughtEquip
    {
        [SerializeField]
        public List<string> playerBoughtItems;
    }

    [System.Serializable]
    public class QuestStars
    {
        [SerializeField]
        public List<int> stars;
    }

    [System.Serializable]
    public class SavedObject
    {
        public PlayerEquipment playerEquipment;
        public PlayerBoughtEquip playerBoughtEquip;
        public QuestStars questStars;
        public int warrior1HP;
        public int warrior2HP;
        public int money;
    }

    #endregion

    public class DataSaver : MonoBehaviour
    {
        #region Singleton & AwakeCalls

        public static DataSaver instance;

        public void AwakeInit(int warriorBaseHp, int numQuests)
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }

            savedObject = new SavedObject();
            ResetSavedObject(warriorBaseHp, 4, 3, numQuests);

            warriorHp = warriorBaseHp;
            questsCount = numQuests;

            savePath = Application.persistentDataPath + "/" + fileName;
            Debug.Log(savePath);
            LoadData();
        }

        #endregion

        private int warriorHp;
        private int questsCount;

        private SavedObject savedObject;
        public string fileName;
        private string savePath;

        private void LateUpdate()
        {
            if(saveDataAtEndFrame)
            {
                string json = JsonConvert.SerializeObject(savedObject, Formatting.Indented); 
                File.WriteAllText(savePath, json);
                saveDataAtEndFrame = false;
            }
        }

        public void ResetSavedObject(int warriorBaseHP, int playerCount, int itemsPerPlayer, int numQuests)
        {
            savedObject.money = 0;
            savedObject.warrior1HP = warriorBaseHP;
            savedObject.warrior2HP = warriorBaseHP;

            int playerItemCount = playerCount * itemsPerPlayer;
            savedObject.playerEquipment = new PlayerEquipment();
            savedObject.playerEquipment.playerEquip = new List<int>(playerItemCount);
            savedObject.playerBoughtEquip = new PlayerBoughtEquip();
            savedObject.playerBoughtEquip.playerBoughtItems = new List<string>(playerItemCount);
            for (int index = 0; index < playerItemCount; index++)
            {
                savedObject.playerEquipment.playerEquip.Add(0);
                savedObject.playerBoughtEquip.playerBoughtItems.Add("0");
            }
            savedObject.questStars = new QuestStars();
            savedObject.questStars.stars = new List<int>(numQuests);
            for (int index = 0; index < numQuests; index++)
            {
                savedObject.questStars.stars.Add(0);
            }
        }

        public void LoadData()
        {
            if (File.Exists(savePath))
            {
                try
                {
                    string contents = File.ReadAllText(savePath);
                    savedObject = JsonConvert.DeserializeObject<SavedObject>(contents);
                }
                catch(Exception ex)
                {
                    Debug.LogError("Could not load json at: " + savePath + "; Message: " + ex.StackTrace);
                }
            }
        }

        public bool saveDataAtEndFrame = false;
        public void SaveModifiedData()
        {
            if (!File.Exists(savePath))
                File.Create(savePath);

            saveDataAtEndFrame = true;
        }


        public void ResetSavedData()
        {
            if (File.Exists(savePath))
                File.Delete(savePath);

            savedObject = new SavedObject();
            ResetSavedObject(warriorHp, 4, 3, questsCount);
            saveDataAtEndFrame = true;
        }

        public void SaveMoney(int data)
        {
            savedObject.money = data;
        }

        public void SaveWarriorHP(int warriorIndex, int hp)
        {
            if(warriorIndex == 0)
                savedObject.warrior1HP = hp;
            else if(warriorIndex == 1)
                savedObject.warrior2HP = hp;
        }

        public void SavePlayerEquipped(PlayerTypes player, int itemIndex, int newItemLevel)
        {
            int index = ((int)player - 1) * 3 + itemIndex;
            savedObject.playerEquipment.playerEquip[index] = newItemLevel;
        }

        public void  SavePlayerBoughtEquip(PlayerTypes player, int itemIndex, int newItemLevel)
        {
            int index = ((int)player - 1) * 3 + itemIndex;
            savedObject.playerBoughtEquip.playerBoughtItems[index] += newItemLevel;
        }

        public void SaveQuestProgress(int questIndex, int stars)
        {
            savedObject.questStars.stars[questIndex] = stars;
        }

        public int GetMoney()
        {
            return savedObject.money;
        }

        public int GetWarriorHP(int warriorIndex)
        {
            if (warriorIndex == 0)
                return savedObject.warrior1HP;
            else if (warriorIndex == 1)
                return savedObject.warrior2HP;
            else
                return 0;
        }

        public int GetPlayerEquippedLevel(PlayerTypes player, int itemIndex)
        {
            int index = ((int)player - 1) * 3 + itemIndex;
            return savedObject.playerEquipment.playerEquip[index];
        }

        public string GetPlayerBoughtEquip(PlayerTypes player, int itemIndex)
        {
            int index = ((int)player - 1) * 3 + itemIndex;
            return savedObject.playerBoughtEquip.playerBoughtItems[index];
        }

        public int GetQuestStars(int questIndex)
        {
            return savedObject.questStars.stars[questIndex];
        }
    }
}