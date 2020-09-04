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

        public TextMeshProUGUI moneyText;

        private DataRetainer dataRetainer;

        private void Start()
        {
            dataRetainer = DataRetainer.instance;
            moneyText.text = "$" + dataRetainer.Money;
        }

        private void Update()
        {
            healthbar1.rectTransform.localScale = new Vector3((float)dataRetainer.Warrior1HP / dataRetainer.warriorMaxHP, 1, 1);
            healthbar2.rectTransform.localScale = new Vector3((float)dataRetainer.Warrior2HP / dataRetainer.warriorMaxHP, 1, 1);
        }
    }
}