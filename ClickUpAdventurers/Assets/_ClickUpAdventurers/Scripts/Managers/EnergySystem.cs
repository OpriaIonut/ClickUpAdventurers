using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class EnergySystem : MonoBehaviour
    {
        [Tooltip("In seconds.")]
        public float timeToFullRecharge;

        [Tooltip("Once the recovery ammount reaches this variable, it will actualize the healthbars.")]
        public int recoverAmmountSteps = 2;

        private float recoveryRate;
        private float recoverAmmount;

        private DataRetainer dataRetainer;
        private void Start()
        {
            dataRetainer = DataRetainer.instance;
        }

        private void FixedUpdate()
        {
            if(dataRetainer.Warrior1HP != dataRetainer.warriorMaxHP || dataRetainer.Warrior2HP != dataRetainer.warriorMaxHP)
            {
                recoveryRate = dataRetainer.warriorMaxHP * 2.0f / timeToFullRecharge;
                recoverAmmount += recoveryRate * Time.fixedDeltaTime;
                if(recoverAmmount > recoverAmmountSteps)
                {
                    int halvedAmmount = (int)recoverAmmount / 2;
                    if(dataRetainer.Warrior1HP != dataRetainer.warriorMaxHP)
                        dataRetainer.Warrior1HP += halvedAmmount;
                    else
                        dataRetainer.Warrior2HP += halvedAmmount;

                    if (dataRetainer.Warrior2HP != dataRetainer.warriorMaxHP)
                        dataRetainer.Warrior2HP += halvedAmmount;
                    else
                        dataRetainer.Warrior1HP += halvedAmmount;

                    recoverAmmount -= 2 * halvedAmmount;
                    dataRetainer.SaveModifiedData();
                }
            }
        }
    }
}