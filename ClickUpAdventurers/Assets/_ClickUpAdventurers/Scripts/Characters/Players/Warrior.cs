using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClickUpAdventurers
{
    public class Warrior : PlayerCharacter
    {
        public Image healthbar;
        public TextMeshProUGUI healthText;

        public List<EnemyCharacter> enemyContactList = new List<EnemyCharacter>();

        public int damage;
        public float attackCooldown = 12;
        public int endWaveRecoveryAmmount = 5;
        public int startingHealth = 100;

        private int health;
        public int Health
        {
            get { return health; }
            set
            {
                //The property was called before any initialization could be done, which will lead to errors
                if (maxHealth == 0)
                    return;

                health = value;
                if (value < 0)
                    Die();

                if (value > maxHealth)
                    health = maxHealth;
                healthText.text = "" + health + " / " + maxHealth;
                healthbar.rectTransform.localScale = new Vector3(1 - (maxHealth - health) / maxHealth, 1, 1);
            }
        }

        public int maxHealth;
        private float healthbarInitVal = 1;
        private float lastAttackTime = 0;

        private void Start()
        {
            InheritedStartCalls();

            maxHealth = health;
            healthbarInitVal = healthbar.rectTransform.localScale.x;
            healthbar.rectTransform.localScale = new Vector3(1 - healthbarInitVal * (maxHealth - health) / maxHealth, 1, 1);
        }

        private void Update()
        {
            if(attackCooldown != 0 && Time.time - lastAttackTime > attackCooldown && enemyContactList.Count != 0)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        public void TakeDamage(int value)
        {
            Health -= value;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            health = 0;
            gameObject.SetActive(false);
        }

        #region Inherited methods

        public override void Attack()
        {
            int index = Random.Range(0, enemyContactList.Count);
            while(enemyContactList[index] == null)
            {
                enemyContactList.RemoveAt(index);
                index = Random.Range(0, enemyContactList.Count);

                if (enemyContactList.Count == 0)
                    return;
            }
            enemyContactList[index].TakeDamage(damage);
        }

        public override void RotateTowardsTouchPos()
        {
            //Intentionally left empty
        }

        #endregion
    }
}