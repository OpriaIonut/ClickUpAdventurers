using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class Warrior : PlayerCharacter
    {
        public Image healthbar;

        public int health = 100;

        private float healthbarInitVal;
        private int healthInitVal;

        private void Start()
        {
            InheritedStartCalls();

            healthInitVal = health;
            healthbarInitVal = healthbar.rectTransform.localScale.x;
        }

        public void TakeDamage(int value)
        {
            health -= value;
            if (health <= 0)
            {
                health = 0;
                gameObject.SetActive(false);
            }
            healthbar.rectTransform.localScale = new Vector3(1 - healthbarInitVal * (healthInitVal - health) / healthInitVal, 1, 1);
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