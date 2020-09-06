using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ClickUpAdventurers
{
    public class ItemUIManager : MonoBehaviour
    {
        [System.Serializable]
        public struct UiItems
        {
            public PlayerItems[] playerItems;
        };

        [Header("UI references")]
        public GameObject contentPanel;
        public GameObject footerPanel;
        public TextMeshProUGUI[] itemNames;
        public TextMeshProUGUI itemDescription;
        public GameObject buyButton;
        public GameObject equipButton;

        [Header("ConfirmationMenu")]
        public GameObject confirmationMenu;
        public TextMeshProUGUI confirmHeader;
        public TextMeshProUGUI confirmContent;

        public List<ItemScriptableObj> allItems;

        private int selectedItemType;
        private int selectedItemIndex;
        private bool selectedConfirmMenu;

        private UiItems[] uiItems;
        private ItemScriptableObj selectedItem;
        private DataRetainer dataRetainer;

        private void Start()
        {
            dataRetainer = DataRetainer.instance;
            contentPanel.SetActive(false);
            footerPanel.SetActive(false);

            confirmationMenu.SetActive(false);
            buyButton.SetActive(false);
            equipButton.SetActive(false);

            SortUiItems();
        }

        private void SortUiItems()
        {
            int playerCount = 4;

            uiItems = new UiItems[playerCount];
            //For all players
            for(int playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                //0 - None type so we need to add 1 to get the actual player
                PlayerTypes playerType = (PlayerTypes)(playerIndex + 1);
                PlayerItems playerItems = dataRetainer.GetPlayerItems(playerType);

                uiItems[playerIndex].playerItems = new PlayerItems[3];
                //For each item effect that the player can have
                for (int itemIndex = 0; itemIndex < playerItems.items.Length; itemIndex++)
                {
                    uiItems[playerIndex].playerItems[itemIndex] = new PlayerItems();
                    //Find the items that can be equiped by the current player and has the desired effect
                    ItemEffect effect = playerItems.items[itemIndex].effect;
                    List<ItemScriptableObj> foundItems = new List<ItemScriptableObj>();
                    for(int allItemsIndex = 0; allItemsIndex < allItems.Count; allItemsIndex++)
                    {
                        if(allItems[allItemsIndex].effect == effect && allItems[allItemsIndex].player == playerType)
                        {
                            foundItems.Add(allItems[allItemsIndex]);
                            allItems.RemoveAt(allItemsIndex);
                            allItemsIndex--;
                        }
                    }
                    //Sort the items to be in ascending order based on multipliers
                    ItemScriptableObj[] itemsToPlace = new ItemScriptableObj[foundItems.Count];
                    for(int itemsToPlaceIndex = 0; itemsToPlaceIndex < itemsToPlace.Length; itemsToPlaceIndex++)
                    {
                        ItemScriptableObj min = foundItems[0];
                        int minIndex = 0;
                        for(int foundItemsIndex = 1; foundItemsIndex < foundItems.Count; foundItemsIndex++)
                        {
                            if (foundItems[foundItemsIndex].multiplier < min.multiplier)
                            {
                                min = foundItems[foundItemsIndex];
                                minIndex = foundItemsIndex;
                            }
                        }
                        itemsToPlace[itemsToPlaceIndex] = min;
                        foundItems.RemoveAt(minIndex);
                    }
                    //Add them to the uiItems
                    uiItems[playerIndex].playerItems[itemIndex].items = itemsToPlace;
                }
            }
        }

        #region OnClick events

        public void BuyButtonClickEvent()
        {
            if (selectedConfirmMenu)
                return;

            if (dataRetainer.Money < selectedItem.price)
                return;

            confirmationMenu.SetActive(true);
            confirmContent.text = selectedItem.itemName + "\n\n" + itemNames[selectedItemType].text + ": x" + selectedItem.multiplier + "\nPrice: $" + selectedItem.price;
            selectedConfirmMenu = true;
        }

        public void EquipButtonClickEvent()
        {
            if (selectedConfirmMenu)
                return;

            EquipItem();
        }

        public void ConfirmBuyButtonClickEvent()
        {
            confirmationMenu.SetActive(false);
            selectedConfirmMenu = false;

            dataRetainer.Money -= selectedItem.price;
            //To do: save to the disk the fact that you bought the item
            EquipItem();
            dataRetainer.SaveModifiedData();
        }

        public void ConfirmCancelButtonClickEvent()
        {
            confirmationMenu.SetActive(false);
            selectedConfirmMenu = false;
        }

        public void BackButtonClickEvent()
        {
            if (selectedConfirmMenu)
            {
                confirmationMenu.SetActive(false);
                selectedConfirmMenu = false;
            }
            else
            {
                SceneLoader.instance.LoadScene("MainScene");
            }
        }

        public void SelectPlayerItemList(int itemListIndex)
        {
            selectedItemType = itemListIndex;
        }

        public void SelectItemIndex(int itemIndex)
        {
            selectedItemIndex = itemIndex;
        }

        #endregion

        public void ApplyItemSelection()
        {
            selectedItem = uiItems[selectedPlayer].playerItems[selectedItemType].items[selectedItemIndex];
            if (selectedItem != null)
            {
                itemDescription.text = selectedItem.itemName + "\n\n" + itemNames[selectedItemType].text + ": x" + selectedItem.multiplier + "\n\nPrice: $" + selectedItem.price;

                //To do: check to see if it is already bought
                buyButton.SetActive(true);
            }
            else
            {
                itemDescription.text = "";
                buyButton.SetActive(false);
                equipButton.SetActive(false);
            }
        }

        private void EquipItem()
        {
            dataRetainer.playerItems[selectedPlayer].items[selectedItemType] = uiItems[selectedPlayer].playerItems[selectedItemType].items[selectedItemIndex];
        }

        private int selectedPlayer;
        public void SelectPlayer(int player)
        {
            if (selectedConfirmMenu)
                return;

            selectedPlayer = player;
            contentPanel.SetActive(true);
            footerPanel.SetActive(true);
            itemDescription.text = "";

            itemNames[0].text = uiItems[player].playerItems[0].items[0].effectName;
            itemNames[1].text = uiItems[player].playerItems[1].items[0].effectName;
            itemNames[2].text = uiItems[player].playerItems[2].items[0].effectName;
        }
    }
}