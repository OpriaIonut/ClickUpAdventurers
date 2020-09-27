using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class EquipmentManager : MonoBehaviour
    {
        public Archer archer;
        public Looter looter;
        public Magician mage;
        public Warrior warrior1;
        public Warrior warrior2;

        public void Start()
        {
            DataRetainer dataRetainer = DataRetainer.instance;

            ItemScriptableObj item1 = dataRetainer.GetEquippedItem(PlayerTypes.Archer, ItemEffect.Damage);
            ItemScriptableObj item2 = dataRetainer.GetEquippedItem(PlayerTypes.Archer, ItemEffect.Cooldown);
            ItemScriptableObj item3 = dataRetainer.GetEquippedItem(PlayerTypes.Archer, ItemEffect.AccuracyTime);

            archer.damage = (int)(archer.damage * item1.multiplier);
            archer.shootCooldownTime = archer.shootCooldownTime / item2.multiplier;
            archer.accuracyMaxTime = archer.accuracyMaxTime / item3.multiplier;

            item1 = dataRetainer.GetEquippedItem(PlayerTypes.Mage, ItemEffect.Size);
            item2 = dataRetainer.GetEquippedItem(PlayerTypes.Mage, ItemEffect.Cooldown);
            item3 = dataRetainer.GetEquippedItem(PlayerTypes.Mage, ItemEffect.Damage);

            mage.ballSizeMultiplier = mage.ballSizeMultiplier * item1.multiplier;
            mage.castTime = mage.castTime / item2.multiplier;
            mage.damage = (int)(mage.damage * item3.multiplier);

            item1 = dataRetainer.GetEquippedItem(PlayerTypes.Looter, ItemEffect.Size);
            item2 = dataRetainer.GetEquippedItem(PlayerTypes.Looter, ItemEffect.Health);
            item3 = dataRetainer.GetEquippedItem(PlayerTypes.Looter, ItemEffect.Cooldown);

            looter.capacity = (int)(looter.capacity * item1.multiplier);
            looter.health = (int)(looter.health * item2.multiplier);
            looter.gatherTime = looter.gatherTime / item3.multiplier;
            looter.movementSpeed = looter.movementSpeed * item3.multiplier;

            item1 = dataRetainer.GetEquippedItem(PlayerTypes.Warrior, ItemEffect.Cooldown);
            item2 = dataRetainer.GetEquippedItem(PlayerTypes.Warrior, ItemEffect.Health);
            item3 = dataRetainer.GetEquippedItem(PlayerTypes.Warrior, ItemEffect.HealthRecovery);

            if (item1.multiplier == 0)
                warrior1.attackCooldown = 0;
            else
                warrior1.attackCooldown = warrior1.attackCooldown / item1.multiplier;
            warrior1.maxHealth = (int)(warrior1.startingHealth * item2.multiplier);
            warrior1.Health = dataRetainer.Warrior1HP;
            warrior1.endWaveRecoveryAmmount = (int)(warrior1.endWaveRecoveryAmmount * item3.multiplier);

            if (item1.multiplier == 0)
                warrior2.attackCooldown = 0;
            else
                warrior2.attackCooldown = warrior2.attackCooldown / item1.multiplier;
            warrior2.maxHealth = (int)(warrior2.startingHealth * item2.multiplier);
            warrior2.Health = dataRetainer.Warrior2HP;
            warrior2.endWaveRecoveryAmmount = (int)(warrior2.endWaveRecoveryAmmount * item3.multiplier);
        }
    }
}