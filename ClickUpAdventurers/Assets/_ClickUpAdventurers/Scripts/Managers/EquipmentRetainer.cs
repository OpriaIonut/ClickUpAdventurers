using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    [System.Serializable]
    public class SortedItems
    {
        /// <summary>
        /// The row specifies the item effect, which can be checked in rowEffect.
        /// The column repesents the item level
        /// </summary>
        public ItemScriptableObj[,] item;
        public ItemEffect[] rowEffect;
    }

    [System.Serializable]
    public class EquippedItems
    {
        public ItemScriptableObj[] item;
    }

    public class EquipmentRetainer : MonoBehaviour
    {
        #region Singleton

        public static EquipmentRetainer instance;

        public void AwakeInit()
        {
            if (instance != null)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        #endregion

        [SerializeField]
        public ItemScriptableObj[] allItems;

        //Will contain the items for all players, sorted by item type and item level
        /// <summary>
        /// The index of the array will specify the player. Use the enum (int)PlayerTypes - 1
        /// </summary>
        private SortedItems[] sortedItems;

        //Currently equipped items by the player
        private EquippedItems[] playerEquippedItems;

        //The key will be string of two digits: first one is the player index, the second one is the effect id (in the enum)
        //The result will be the row index onto which we need to set the item
        //This index will be distribuited to the effects based on first come first served
        private Dictionary<string, int> playerEffectTargetIndex;

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

        private DataRetainer dataRetainer;
        private DataSaver dataSaver;

        #region Init

        public void Init()
        {
            dataSaver = DataSaver.instance;
            dataRetainer = DataRetainer.instance;

            SortItems();
            playerEquippedItems = new EquippedItems[4];
            for (int index = 0; index < 4; index++)
            {
                playerEquippedItems[index] = new EquippedItems();
                playerEquippedItems[index].item = new ItemScriptableObj[3];
            }

            effectNames = new string[4, 3];
            for (int index = 0; index < 4; index++)
            {
                for (int index2 = 0; index2 < 3; index2++)
                {
                    PlayerTypes playerIndex = (PlayerTypes)(index + 1);
                    effectNames[index, index2] = GetItem(playerIndex, playerEffects[index, index2], 0).effectName;
                }
            }
        }

        public void LoadEquippedItems()
        {
            for (int playerIndex = 0; playerIndex < 4; playerIndex++)
            {
                for (int effectIndex = 0; effectIndex < 3; effectIndex++)
                {
                    int result = dataSaver.GetPlayerEquippedLevel((PlayerTypes)(playerIndex + 1), effectIndex);
                    EquipItem((PlayerTypes)(playerIndex + 1), playerEffects[playerIndex, effectIndex], result);
                }
            }
        }

        //Called in awake, it sorts the items set in the inspector based on player types, item effect and level
        private void SortItems()
        {
            sortedItems = new SortedItems[4];
            for (int index = 0; index < 4; index++)
            {
                sortedItems[index] = new SortedItems();
                sortedItems[index].item = new ItemScriptableObj[3, 4];
                sortedItems[index].rowEffect = new ItemEffect[3];
            }

            //The key will be string of two digits: first one is the player index, the second one is the effect id (in the enum)
            //The result will be the row index onto which we need to set the item
            //This index will be distribuited to the effects based on first come first served
            playerEffectTargetIndex = new Dictionary<string, int>();

            //Reached index will retain how many effects we found for each player in turn
            int[] reachedIndex = new int[4];
            for (int index = 0; index < allItems.Length; index++)
            {
                int playerIndex = (int)allItems[index].player - 1;
                int effectId = (int)allItems[index].effect;
                int itemLevel = (int)allItems[index].itemLevel;
                string key = playerIndex.ToString() + effectId.ToString();

                int rowIndex;
                //If we found the key in the dictionary, it means that the effect is already added onto the sorted list, so find it's index from the dictionary
                if (playerEffectTargetIndex.ContainsKey(key))
                {
                    rowIndex = playerEffectTargetIndex[key];
                }
                else
                {
                    //If it isn't in the dictionary, add it to the dictionary
                    playerEffectTargetIndex.Add(key, reachedIndex[playerIndex]);
                    rowIndex = reachedIndex[playerIndex];
                    reachedIndex[playerIndex]++;
                }
                //Set the item and effect to the desired positions
                sortedItems[playerIndex].item[rowIndex, itemLevel] = allItems[index];
                sortedItems[playerIndex].rowEffect[rowIndex] = allItems[index].effect;
            }
        }

        #endregion

        #region Getters

        private int GetEffectRowIndex(PlayerTypes player, ItemEffect effect)
        {
            int playerIndex = (int)player - 1;
            for (int index = 0; index < 3; index++)
                if (sortedItems[playerIndex].rowEffect[index] == effect)
                    return index;
            return -1;
        }

        public ItemScriptableObj GetEquippedItem(PlayerTypes player, ItemEffect effect)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);
            return playerEquippedItems[playerIndex].item[rowIndex];
        }

        public ItemScriptableObj GetEquippedItem(PlayerTypes player, int effectIndex)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, playerEffects[playerIndex, effectIndex]);
            return playerEquippedItems[playerIndex].item[rowIndex];
        }

        public ItemScriptableObj GetItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);

            return sortedItems[playerIndex].item[rowIndex, itemLevel];
        }

        public ItemScriptableObj GetItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            return GetItem(player, playerEffects[playerIndex, effectIndex], itemLevel);
        }


        public string CheckBoughtItem(PlayerTypes player, int effectIndex)
        {
            return dataSaver.GetPlayerBoughtEquip(player, effectIndex);
        }

        #endregion

        #region Setters

        private void SetEquippedItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);


            if(player == PlayerTypes.Warrior)
            {
                ItemScriptableObj itemToEquip = sortedItems[playerIndex].item[rowIndex, itemLevel];
                dataRetainer.warriorMaxHP = (int)(itemToEquip.multiplier * dataRetainer.warriorBaseHP);
                if (playerEquippedItems[playerIndex].item[rowIndex] != null)
                {
                    float diffFact1 = dataRetainer.Warrior1HP / (playerEquippedItems[playerIndex].item[rowIndex].multiplier * dataRetainer.warriorBaseHP);
                    dataRetainer.Warrior1HP = (int)(dataRetainer.warriorBaseHP * itemToEquip.multiplier * diffFact1);

                    float diffFact2 = dataRetainer.Warrior2HP / (playerEquippedItems[playerIndex].item[rowIndex].multiplier * dataRetainer.warriorBaseHP);
                    dataRetainer.Warrior2HP = (int)(dataRetainer.warriorBaseHP * itemToEquip.multiplier * diffFact2);
                }
            }
            playerEquippedItems[playerIndex].item[rowIndex] = sortedItems[playerIndex].item[rowIndex, itemLevel];
        }

        public void BuyItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            dataSaver.SavePlayerBoughtEquip(player, effectIndex, itemLevel);
        }

        public void EquipItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int effectIndex = -1;
            for (int index = 0; index < 3; index++)
            {
                if (playerEffects[playerIndex, index] == effect)
                {
                    effectIndex = index;
                }
            }

            if (effectIndex != -1)
            {
                SetEquippedItem(player, effect, itemLevel);
                dataSaver.SavePlayerEquipped(player, effectIndex, itemLevel);
            }
        }

        public void EquipItem(PlayerTypes player, int effectIndex, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            SetEquippedItem(player, playerEffects[playerIndex, effectIndex], itemLevel);
            dataSaver.SavePlayerEquipped(player, effectIndex, itemLevel);
        }

        #endregion

        public void ResetAll()
        {
            for(int index = 1; index <= 4; index++)
            {
                PlayerTypes player = (PlayerTypes)index;
                for(int equipIndex = 0; equipIndex < 3; equipIndex++)
                {
                    EquipItem(player, playerEffects[index - 1, equipIndex], 0);
                }    
            }
        }
    }
}