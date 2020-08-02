using UnityEngine;

namespace ClickUpAdventurers
{
    //Base class for the enemies, in the future it may be used as an inherited class by multiple enemy classes
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyCharacter : Character
    {
        public int health = 1;
        public float speed = 3;
        public int damage = 1;
        public float attackCooldown = 1.0f;

        private float lastAttackTime;

        private Rigidbody rb;
        private Warrior collidedWarrior;    //The warrior that we collided with, used to stop movement and other logic

        private BattleManager battleManager;

        private void Start()
        {
            battleManager = BattleManager.instance;
            rb = GetComponent<Rigidbody>();

            lastAttackTime = Time.time;
        }

        private void Update()
        {
            if (!battleManager.GameHasEnded)
            {
                //If the game hasn't ended and we collided with an enemy then damage it at certain time intervals
                if (collidedWarrior != null && Time.time - lastAttackTime > attackCooldown)
                {
                    collidedWarrior.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                //if the game has ended, stop the enemy's movement to simulate a pause functionality
                rb.velocity = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            if (!battleManager.GameHasEnded)
            {
                if (collidedWarrior == null)
                {
                    //If the game hasn't ended and we haven't collided with a warrior then move the enemy towards it's goal. At the moment the goal is at the origin, and we are using Vector3.up so that they stay on the same level as the players
                    Vector3 targetPos = Vector3.up - transform.position;
                    transform.LookAt(Vector3.up);
                    rb.velocity = targetPos.normalized * speed * Time.fixedDeltaTime;
                }
            }
        }

        //Called by other scripts (like the arrow)
        public void TakeDamage(int ammount)
        {
            health -= ammount;
            if (health <= 0)
                Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "Player" && collidedWarrior == null)
            {
                //If we collided with a player, check to see if it is a warrior
                collidedWarrior = other.transform.root.GetComponent<Warrior>();
                if(collidedWarrior != null)
                    rb.velocity = Vector3.zero; //If it is, then stop all movement
            }
        }

        //Inherited calls that don't need implementation yet
        public override void InheritedAwakeCalls()
        {
            throw new System.NotImplementedException();
        }

        public override void InheritedStartCalls()
        {
            throw new System.NotImplementedException();
        }

        public override void InheritedUpdateCalls()
        {
            throw new System.NotImplementedException();
        }
    }
}