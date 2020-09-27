using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    [System.Serializable]
    public enum PlayerTypes
    {
        None,
        Archer,
        Mage,
        Looter,
        Warrior
    };

    [System.Serializable]
    public class PlayerItems
    {
        public ItemScriptableObj[] items;
    };

    // Responsible for holding data throughout the game and to manage calls to the DataSaver
    public class DataRetainer : MonoBehaviour
    {
        #region Singleton & Awake calls

        public static DataRetainer instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            questStars = new Dictionary<string, int>();
            dataSaver = GetComponent<DataSaver>();
            equipRetainter = GetComponent<EquipmentRetainer>();

            equipRetainter.Init();
            LoadStartingData();
        }

        #endregion

        #region Properties & Data Accessors

        private int money;
        public int Money
        {
            get { return money; }
            set { money = value; dataSaver.SetSaveData("money", value); }
        }

        private int warrior1HP;
        public int Warrior1HP
        {
            get { return warrior1HP; }
            set 
            { 
                warrior1HP = value;
                if (warrior1HP > warriorMaxHP)
                    warrior1HP = warriorMaxHP;
                dataSaver.SetSaveData("warrior1HP", warrior1HP); 
            }
        }
        private int warrior2HP;
        public int Warrior2HP
        {
            get { return warrior2HP; }
            set
            {
                warrior2HP = value;
                if (warrior2HP > warriorMaxHP)
                    warrior2HP = warriorMaxHP;
                dataSaver.SetSaveData("warrior2HP", warrior2HP);
            }
        }

        private Dictionary<string, int> questStars;
        public int GetQuestStars(string questName)
        {
            if (questStars.ContainsKey(questName))
                return questStars[questName];
            return 0;
        }
        public bool SaveQuestStars(string questName, int stars, out int previousStars)
        {
            if(questStars.ContainsKey(questName))
            {
                if (questStars[questName] < stars)
                {
                    previousStars = questStars[questName];
                    questStars[questName] = stars;
                    dataSaver.SetSaveData(questName, questStars[questName]);
                    return true;
                }
                else
                {
                    previousStars = stars;
                    return false;
                }
            }
            else
            {
                questStars[questName] = stars;
                previousStars = 0;
                dataSaver.SetSaveData(questName, questStars[questName]);
                return true;
            }
        }

        #endregion

        [Space]
        public int warriorMaxHP = 100;
        public QuestScriptableObj[] allQuests;

        private EquipmentRetainer equipRetainter;
        private DataSaver dataSaver;

        /// <summary>
        /// Use (int)PlayerTypes - 1 for indexing
        /// </summary>
        private ItemEffect[,] playerEffects = new ItemEffect[4, 3]
        {
            { ItemEffect.AccuracyTime,  ItemEffect.Cooldown,    ItemEffect.Damage },
            { ItemEffect.Size,          ItemEffect.Cooldown,    ItemEffect.Damage },
            { ItemEffect.Size,          ItemEffect.Cooldown,    ItemEffect.Health },
            { ItemEffect.Cooldown,      ItemEffect.Health,      ItemEffect.HealthRecovery }
        };
        public string[,] effectNames;

        private void Start()
        {
            effectNames = new string[4, 3];
            for(int index = 0; index < 4; index++)
            {
                for(int index2 = 0; index2 < 3; index2++)
                {
                    PlayerTypes playerIndex = (PlayerTypes)(index + 1);
                    effectNames[index, index2] = equipRetainter.GetItem(playerIndex, playerEffects[index, index2], 0).effectName;
                }
            }
        }

        private void LoadStartingData()
        {
            //dataSaver.ResetSavedData();
            Money = 1;
            LoadEquippedItems();

            ItemScriptableObj warriorHpItem = GetEquippedItem(PlayerTypes.Warrior, ItemEffect.Health);
            warriorMaxHP = (int)(warriorMaxHP * warriorHpItem.multiplier);

            money = dataSaver.LoadInt("money");
            if (warrior1HP == dataSaver.intDefault)
                Warrior1HP = 0;
            warrior1HP = dataSaver.LoadInt("warrior1HP");
            if (warrior1HP == dataSaver.intDefault)
                Warrior1HP = warriorMaxHP;
            warrior2HP = dataSaver.LoadInt("warrior2HP");
            if (warrior2HP == dataSaver.intDefault)
                Warrior2HP = warriorMaxHP;

            foreach (QuestScriptableObj quest in allQuests)
            {
                questStars[quest.title] = dataSaver.LoadInt(quest.title);
            }

            dataSaver.SaveModifiedData();
        }

        public void SaveModifiedData()
        {
            dataSaver.SaveModifiedData();
        }

        public void BuyItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int effectId = (int)playerEffects[playerIndex, effectIndex];
            string key = "bought" + playerIndex.ToString() + effectId.ToString();
            string alreadyBought = dataSaver.LoadString(key);
            if (alreadyBought == dataSaver.stringDefault)
                alreadyBought = "";
            alreadyBought += itemLevel;
            dataSaver.SetSaveData(key, alreadyBought);
        }

        public string CheckBoughtItem(PlayerTypes player, int effectIndex)
        {
            int playerIndex = (int)player - 1;
            int effectId = (int)playerEffects[playerIndex, effectIndex];
            string key = "bought" + playerIndex.ToString() + effectId.ToString();
            return dataSaver.LoadString(key);
        }

        public ItemScriptableObj GetEquippedItem(PlayerTypes player, ItemEffect effect)
        {
            return equipRetainter.GetEquippedItem(player, effect);
        }

        public ItemScriptableObj GetEquippedItem(PlayerTypes player, int effectIndex)
        {
            int playerIndex = (int)player - 1;
            return equipRetainter.GetEquippedItem(player, playerEffects[playerIndex, effectIndex]);
        }

        public ItemScriptableObj GetItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            return equipRetainter.GetItem(player, playerEffects[playerIndex, effectIndex], itemLevel);
        }

        public void EquipItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            equipRetainter.SetEquippedItem(player, effect, itemLevel);
            string saveKey = playerIndex.ToString() + ((int)effect).ToString();
            dataSaver.SetSaveData(saveKey, itemLevel);
        }

        public void EquipItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            equipRetainter.SetEquippedItem(player, playerEffects[playerIndex, effectIndex], itemLevel);
            string saveKey = playerIndex.ToString() + ((int)playerEffects[playerIndex, effectIndex]).ToString();
            dataSaver.SetSaveData(saveKey, itemLevel);
        }

        public void LoadEquippedItems()
        {
            for(int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                for(int effectIndex = 0; effectIndex < 3; effectIndex++)
                {
                    string key = playerIndex.ToString() + ((int)playerEffects[playerIndex, effectIndex]).ToString();
                    int result = dataSaver.LoadInt(key);

                    PlayerTypes playerType = (PlayerTypes)(playerIndex + 1);
                    if (result == dataSaver.intDefault)
                    {
                        EquipItem(playerType, playerEffects[playerIndex, effectIndex], 0);
                        BuyItem(playerType, effectIndex, 0);
                    }
                    else
                        EquipItem(playerType, playerEffects[playerIndex, effectIndex], result);
                }
            }
        }
    }
}