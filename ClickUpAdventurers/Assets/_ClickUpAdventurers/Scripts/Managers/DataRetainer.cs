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

        [Tooltip("Is indexed by the PlayerTypes enum: 0 - Archer, 1 - Mage, 2 - Looter, 3 - Warrior")]
        public PlayerItems[] playerItems;

        [Space]
        public int warriorMaxHP = 100;
        public QuestScriptableObj[] allQuests;

        private DataSaver dataSaver;

        private void LoadStartingData()
        {
            ItemScriptableObj warriorHpItem = GetItem(PlayerTypes.Warrior, ItemEffect.Health);
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

        public ItemScriptableObj GetItem(PlayerTypes player, ItemEffect effect)
        {
            PlayerItems items = GetPlayerItems(player);
            foreach (ItemScriptableObj item in items.items)
                if (item.effect == effect)
                    return item;
            return null;
        }

        public PlayerItems GetPlayerItems(PlayerTypes player)
        {
            if (player == PlayerTypes.None)
                return null;

            return playerItems[(int)player - 1];
        }
    }
}