using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class ItemUIManager : MonoBehaviour
    {
        [Header("UI references")]
        public GameObject contentPanel;
        public GameObject footerPanel;
        public TextMeshProUGUI[] itemNames;
        public Image selectedItemImage;
        public TextMeshProUGUI itemDescription;
        public GameObject buyButton;
        public GameObject equipButton;

        [Header("ConfirmationMenu")]
        public GameObject confirmationMenu;
        public TextMeshProUGUI confirmHeader;
        public TextMeshProUGUI confirmContent;

        private PlayerTypes selectedPlayer;
        private int selectedEffectIndex;
        private int selectedItemLevel;
        private bool selectedConfirmMenu;

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

            selectedItemImage.enabled = false;
        }

        #region OnClick events

        public void BuyButtonClickEvent()
        {
            if (selectedConfirmMenu)
                return;

            if (dataRetainer.Money < selectedItem.price)
                return;

            confirmationMenu.SetActive(true);
            confirmContent.text = selectedItem.itemName + "\n\n" + itemNames[selectedEffectIndex].text + ": x" + selectedItem.multiplier + "\nPrice: $" + selectedItem.price;
            selectedConfirmMenu = true;
        }

        public void EquipButtonClickEvent()
        {
            if (selectedConfirmMenu)
                return;

            EquipItem();
            dataRetainer.SaveModifiedData();
        }

        public void ConfirmBuyButtonClickEvent()
        {
            confirmationMenu.SetActive(false);
            selectedConfirmMenu = false;

            dataRetainer.Money -= selectedItem.price;
            dataRetainer.BuyItem(selectedPlayer, selectedEffectIndex, selectedItemLevel);
            EquipItem();
            dataRetainer.SaveModifiedData();

            equipButton.SetActive(true);
            buyButton.SetActive(false);
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
            selectedEffectIndex = itemListIndex;
        }

        public void SelectItemIndex(int itemIndex)
        {
            selectedItemLevel = itemIndex;
        }

        #endregion

        public void ApplyItemSelection()
        {
            selectedItem = dataRetainer.GetItem(selectedPlayer, selectedEffectIndex, selectedItemLevel);
            if (selectedItem != null)
            {
                itemDescription.text = selectedItem.itemName + "\n\n" + itemNames[selectedEffectIndex].text + ": x" + selectedItem.multiplier + "\n\nPrice: $" + selectedItem.price;
                selectedItemImage.enabled = true;
                selectedItemImage.sprite = selectedItem.uiSprite;

                string boughtStr = dataRetainer.CheckBoughtItem(selectedPlayer, selectedEffectIndex);

                string strSearch = selectedItemLevel.ToString();
                if (boughtStr.Contains(strSearch))
                {
                    equipButton.SetActive(true);
                    buyButton.SetActive(false);
                }
                else
                {
                    equipButton.SetActive(false);
                    buyButton.SetActive(true);
                }
            }
            else
            {
                itemDescription.text = "";
                buyButton.SetActive(false);
                equipButton.SetActive(false);
                selectedItemImage.enabled = false;
            }
        }

        private void EquipItem()
        {
            dataRetainer.EquipItem(selectedPlayer, selectedEffectIndex, selectedItemLevel);
        }

        public void SelectPlayer(int player)
        {
            if (selectedConfirmMenu)
                return;

            selectedPlayer = (PlayerTypes)(player + 1);
            contentPanel.SetActive(true);
            footerPanel.SetActive(true);
            itemDescription.text = "";

            int playerIndex = player;
            itemNames[0].text = dataRetainer.effectNames[playerIndex, 0];
            itemNames[1].text = dataRetainer.effectNames[playerIndex, 1];
            itemNames[2].text = dataRetainer.effectNames[playerIndex, 2];
        }
    }
}