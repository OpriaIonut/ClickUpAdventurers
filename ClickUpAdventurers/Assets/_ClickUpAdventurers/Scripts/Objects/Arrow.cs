using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Arrow : RangedAttackObject
    {
        public int damage = 1;

        bool collided = false;
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy" && collided == false)
            {
                //If we collided with an enemy then damage him and destroy the arrow
                EnemyCharacter enemy = other.gameObject.GetComponent<EnemyCharacter>();
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                collided = true;
            }
        }
    }
}