using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    // Responsible for holding data throughout the game and to manage calls to the DataSaver
    public class DataRetainer : MonoBehaviour
    {
        #region Singleton & Awake calls

        public static DataRetainer instance;

        public void AwakeInit()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }

            questStars = new int[allQuests.Length];
            dataSaver = GetComponent<DataSaver>();
            equipRetainter = GetComponent<EquipmentRetainer>();

            LoadStartingData();
        }

        #endregion

        #region Properties & Data Accessors

        private int money;
        public int Money
        {
            get { return money; }
            set { money = value; dataSaver.SaveMoney(value); }
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
                dataSaver.SaveWarriorHP(0, warrior1HP); 
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
                dataSaver.SaveWarriorHP(1, warrior2HP);
            }
        }

        private int FindQuestIndex(string questName)
        {
            int questIndex = -1;
            for (int index = 0; index < allQuests.Length; index++)
                if (questName == allQuests[index].title)
                {
                    questIndex = index;
                    break;
                }

            return questIndex;
        }

        private int[] questStars;
        public int GetQuestStars(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            if (questIndex == -1)
            {
                Debug.LogError("Could not find quest: " + questName);
                return 0;
            }
            return questStars[questIndex];
        }
        public bool SaveQuestStars(string questName, int stars, out int previousStars)
        {
            int questIndex = FindQuestIndex(questName);
            if (questIndex == -1)
            {
                Debug.LogError("Could not find quest: " + questName);
                previousStars = 0;
                return false;
            }

            previousStars = dataSaver.GetQuestStars(questIndex);
            if (stars > previousStars)
            {
                questStars[questIndex] = stars;
                dataSaver.SaveQuestProgress(questIndex, stars);
            }
            return true;
        }

        #endregion

        public int warriorBaseHP = 100;
        [HideInInspector] public int warriorMaxHP;
        public QuestScriptableObj[] allQuests;

        private EquipmentRetainer equipRetainter;
        private DataSaver dataSaver;

        private void LoadStartingData()
        {
            equipRetainter.Init();
            equipRetainter.LoadEquippedItems();

            ItemScriptableObj warriorHpItem = equipRetainter.GetEquippedItem(PlayerTypes.Warrior, ItemEffect.Health);
            warriorMaxHP = (int)(warriorBaseHP * warriorHpItem.multiplier);

            money = dataSaver.GetMoney();
            warrior1HP = dataSaver.GetWarriorHP(0);
            warrior2HP = dataSaver.GetWarriorHP(1);

            foreach (QuestScriptableObj quest in allQuests)
            {
                int questIndex = FindQuestIndex(quest.title);
                questStars[questIndex] = dataSaver.GetQuestStars(questIndex);
            }

            dataSaver.SaveModifiedData();
        }

        public void SaveModifiedData()
        {
            dataSaver.SaveModifiedData();
        }

        public void ResetAll()
        {
            equipRetainter.ResetAll();
            dataSaver.ResetSavedData();

            Money = 0;
            Warrior1HP = warriorMaxHP;
            Warrior2HP = warriorMaxHP;
            questStars = new int[allQuests.Length];
        }
    }
}