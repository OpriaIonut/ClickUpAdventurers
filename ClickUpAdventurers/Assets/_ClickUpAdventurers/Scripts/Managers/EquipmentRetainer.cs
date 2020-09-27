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
        [SerializeField]
        private ItemScriptableObj[] allItems;

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

        public void Init()
        {
            SortItems();
            playerEquippedItems = new EquippedItems[4];
            for (int index = 0; index < 4; index++)
            {
                playerEquippedItems[index] = new EquippedItems();
                playerEquippedItems[index].item = new ItemScriptableObj[3];
            }
        }

        public ItemScriptableObj GetEquippedItem(PlayerTypes player, ItemEffect effect)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);
            return playerEquippedItems[playerIndex].item[rowIndex];
        }

        public void SetEquippedItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);

            playerEquippedItems[playerIndex].item[rowIndex] = sortedItems[playerIndex].item[rowIndex, itemLevel];
        }

        private int GetEffectRowIndex(PlayerTypes player, ItemEffect effect)
        {
            int playerIndex = (int)player - 1;
            for (int index = 0; index < 3; index++)
                if (sortedItems[playerIndex].rowEffect[index] == effect)
                    return index;
            return -1;
        }

        public ItemScriptableObj GetItem(PlayerTypes player, ItemEffect effect, int itemLevel)
        {
            int playerIndex = (int)player - 1;
            int rowIndex = GetEffectRowIndex(player, effect);

            return sortedItems[playerIndex].item[rowIndex, itemLevel];
        }

        //Called in awake, it sorts the items set in the inspector based on player types, item effect and level
        private void SortItems()
        {
            sortedItems = new SortedItems[4];
            for(int index = 0; index < 4; index++)
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
            for(int index = 0; index < allItems.Length; index++)
            {
                int playerIndex = (int)allItems[index].player - 1;
                int effectId = (int)allItems[index].effect;
                int itemLevel = (int)allItems[index].itemLevel;
                string key = playerIndex.ToString() + effectId.ToString();

                int rowIndex;
                //If we found the key in the dictionary, it means that the effect is already added onto the sorted list, so find it's index from the dictionary
                if(playerEffectTargetIndex.ContainsKey(key))
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
    }
}