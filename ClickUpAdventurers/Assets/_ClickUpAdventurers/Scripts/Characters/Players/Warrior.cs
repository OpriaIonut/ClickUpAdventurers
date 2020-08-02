using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class Warrior : PlayerCharacter
    {
        public int health = 100;

        private void Start()
        {
            InheritedStartCalls();
        }

        public void TakeDamage(int value)
        {
            health -= value;
            if (health <= 0)
                Destroy(gameObject);
        }

        #region Inherited methods

        public override void Attack()
        {
            //Intentionally left empty
        }

        public override void RotateTowardsTouchPos()
        {
            //Intentionally left empty
        }

        #endregion
    }
}