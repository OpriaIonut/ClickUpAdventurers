using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public enum ItemEffect
    {
        None,
        Damage,
        Cooldown,
        AccuracyTime,
        Size,
        Health,
        HealthRecovery
    };

    [CreateAssetMenu(fileName = "ItemTemplate", menuName = "ScriptableObjects/ItemScriptable")]
    public class ItemScriptableObj : ScriptableObject
    {
        public string itemName;
        public PlayerTypes player;
        public ItemEffect effect;
        public int itemLevel;
        public float multiplier;
        public int price;
        public Sprite uiSprite;

        /// <summary>
        /// Name displayed on the UI, the enum can't be used for this because it is more general
        /// </summary>
        public string effectName;
    }
}