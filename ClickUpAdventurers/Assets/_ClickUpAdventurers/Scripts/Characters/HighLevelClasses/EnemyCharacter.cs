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

        public GameObject lootPrefab;

        //The position towards which it moves. If the looter is away from the party, he will be the main target
        [HideInInspector] public Vector3 targetPos = Vector3.zero;
        private Vector3 targetPosBackup;

        private float lastAttackTime;

        private Rigidbody rb;
        private Looter looter;              //Reference to the looter script to know when he is away from the party
        private Warrior collidedWarrior;    //The warrior that we collided with, used to stop movement and other logic

        private BattleManager battleManager;

        private bool damageLooter = false;

        private void Start()
        {
            battleManager = BattleManager.instance;
            rb = GetComponent<Rigidbody>();
            looter = Resources.FindObjectsOfTypeAll<Looter>()[0];
            lastAttackTime = Time.time;
        }

        private void Update()
        {
            if (!battleManager.GameHasEnded && !battleManager.GamePaused)
            {
                //If the game hasn't ended and we collided with an enemy then damage it at certain time intervals
                if (collidedWarrior != null && Time.time - lastAttackTime > attackCooldown)
                {
                    collidedWarrior.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
                //If we collided with the looter then attack him
                if(damageLooter && Time.time - lastAttackTime > attackCooldown)
                {
                    looter.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }

                //We want only the enemies from the same side on the map that he gathers on to attack him
                bool locationCond = (looter.transform.position.x > 0 && transform.position.x > 0) || (looter.transform.position.x < 0 && transform.position.x < 0);

                //If the looter is alive and away from the party then move towards him
                if (looter.gameObject.activeSelf && (looter.gatherLoot || looter.returnToParty) && locationCond)
                {
                    targetPos = looter.transform.position;
                }
                else
                {
                    targetPos = targetPosBackup;
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
            if (!battleManager.GameHasEnded && !battleManager.GamePaused)
            {
                if (collidedWarrior == null || !collidedWarrior.gameObject.activeSelf)
                {
                    //If the game hasn't ended and we haven't collided with a warrior then move the enemy towards it's goal. At the moment the goal is at the origin, and we are using Vector3.up so that they stay on the same level as the players
                    Vector3 direction = targetPos - transform.position;

                    Vector3 lookPos = targetPos - transform.position;
                    lookPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(lookPos);
                    rb.velocity = direction.normalized * speed * Time.fixedDeltaTime;
                }
            }
        }

        public void SetTarget(Vector3 pos)
        {
            targetPos = pos;
            targetPosBackup = pos;
        }

        private void DropLoot()
        {
            Instantiate(lootPrefab).transform.position = transform.position;
        }

        //Called by other scripts (like the arrow)
        public void TakeDamage(int ammount)
        {
            health -= ammount;
            if (health <= 0)
            {
                DropLoot();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "Player" && collidedWarrior == null)
            {
                //If we collided with a player, check to see if it is a warrior
                collidedWarrior = other.transform.root.GetComponent<Warrior>();
                if(collidedWarrior != null)
                    rb.velocity = Vector3.zero; //If it is, then stop all movement
                else
                {
                    //If we collided with the looter then start damaging him
                    if(other.transform.root.GetComponent<Looter>())
                    {
                        damageLooter = true;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.GetComponent<Looter>())
            {
                damageLooter = false;
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