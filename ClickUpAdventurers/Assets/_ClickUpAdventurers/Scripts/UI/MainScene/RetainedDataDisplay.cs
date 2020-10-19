using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class RetainedDataDisplay : MonoBehaviour
    {
        public Image healthbar1;
        public Image healthbar2;

        public TextMeshProUGUI healthbarText1;
        public TextMeshProUGUI healthbarText2;

        public TextMeshProUGUI moneyText;

        public GameObject optionsCanvas;
        public GameObject resetProgressConfirmCanvas;

        private EquipmentRetainer equipmentRetainer;
        private DataRetainer dataRetainer;

        private bool showOptionsCanvas = false;

        private void Start()
        {
            equipmentRetainer = EquipmentRetainer.instance;
            dataRetainer = DataRetainer.instance;
            moneyText.text = "$" + dataRetainer.Money;

            optionsCanvas.SetActive(false);
            resetProgressConfirmCanvas.SetActive(false);
        }

        private void Update()
        {
            healthbar1.rectTransform.localScale = new Vector3((float)dataRetainer.Warrior1HP / dataRetainer.warriorMaxHP, 1, 1);
            healthbar2.rectTransform.localScale = new Vector3((float)dataRetainer.Warrior2HP / dataRetainer.warriorMaxHP, 1, 1);

            healthbarText1.text = "" + dataRetainer.Warrior1HP + " / " + dataRetainer.warriorMaxHP;
            healthbarText2.text = "" + dataRetainer.Warrior2HP + " / " + dataRetainer.warriorMaxHP;
        }

        #region ClickEvents

        public void OnClick_ToggleOptionsCanvas()
        {
            showOptionsCanvas = !showOptionsCanvas;
            optionsCanvas.SetActive(showOptionsCanvas);
            resetProgressConfirmCanvas.SetActive(false);
        }

        public void OnClick_ResetAllProgress()
        {
            resetProgressConfirmCanvas.SetActive(true);
        }

        public void OnClick_ConfirmResetAllProgress()
        {
            dataRetainer.ResetAll();
            resetProgressConfirmCanvas.SetActive(false);
        }

        public void OnClick_CancelResetAllProgress()
        {
            resetProgressConfirmCanvas.SetActive(false);
        }

        #endregion
    }
}