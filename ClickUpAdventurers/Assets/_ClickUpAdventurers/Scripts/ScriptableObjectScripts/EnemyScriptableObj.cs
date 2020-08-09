using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    [CreateAssetMenu(fileName = "EnemyTemplate", menuName = "ScriptableObjects/EnemyScriptable")]
    public class EnemyScriptableObj : ScriptableObject
    {
        public string name;
        public int health = 1;
        public float speed = 50;
        public int damage = 1;
        public float attackCooldown = 1.0f;
        public GameObject lootPrefab;
    }
}