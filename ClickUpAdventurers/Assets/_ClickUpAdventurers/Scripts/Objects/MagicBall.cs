using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class MagicBall : RangedAttackObject
    {
        public int damage = 1;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                //If we collided with an enemy then damage him and destroy the arrow
                EnemyCharacter enemy = other.gameObject.GetComponent<EnemyCharacter>();
                enemy.TakeDamage(damage);
                //The purpose of the magic ball is that it is big and destroys all enemies it comes into contact with so we delay the destroy for a short while in order to kill all enemies it has collided with
                Destroy(gameObject, 0.1f);
            }
        }
    }
}