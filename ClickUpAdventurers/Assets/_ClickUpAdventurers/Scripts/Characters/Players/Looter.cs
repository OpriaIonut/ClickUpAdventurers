using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class Looter : PlayerCharacter
    {
        public float gatherTime = 1.0f;
        public int capacity = 5;
        public int movementSpeed = 5;
        public int health = 3;

        public Image healthbar;

        private int gatheredLoot = 0;
        [HideInInspector] public bool gatherLoot = false;
        [HideInInspector] public bool returnToParty = false;

        private float lootPickupTime = 0;
        private List<GameObject> lootToPick;
        private Vector3 returnPos;
        private Quaternion initRot;

        private int healthInitVal;
        private float healthbarInitVal;

        private void Start()
        {
            base.InheritedStartCalls();

            initRot = transform.rotation;

            healthInitVal = health;
            healthbarInitVal = healthbar.rectTransform.localScale.x;
        }

        private void Update()
        {
            base.InheritedUpdateCalls();

            if (Time.time - lootPickupTime > gatherTime)
            {
                GatherLoot();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.root.tag == "Loot")
            {
                PickLoot(other.transform.root.gameObject);
            }
        }

        private void GatherLoot()
        {
            if (gatherLoot)
            {
                while (lootToPick.Count != 0 && lootToPick[0] == null)
                    lootToPick.RemoveAt(0);

                if (lootToPick.Count == 0)
                {
                    returnToParty = true;
                    gatherLoot = false;
                }
                else
                {
                    Vector3 moveTowards = Vector3.MoveTowards(transform.position, lootToPick[0].transform.position, Time.deltaTime * movementSpeed);
                    transform.position = moveTowards;

                    Vector3 lookPos = lootToPick[0].transform.position - transform.position;
                    lookPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(lookPos);
                }
            }
            if (returnToParty)
            {
                Vector3 moveTowards = Vector3.MoveTowards(transform.position, returnPos, Time.deltaTime * movementSpeed);
                transform.position = moveTowards;
                if (transform.position == returnPos)
                {
                    EndSearch();
                    return;
                }

                Vector3 lookPos = returnPos - transform.position;
                lookPos.y = 0;
                transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }

        private void EndSearch()
        {
            BattleManager.instance.gatheredLoot += gatheredLoot;
            gatheredLoot = 0;
            returnToParty = false;
            transform.rotation = initRot;
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

        private void PickLoot(GameObject loot)
        {
            for(int index = 0; index < lootToPick.Count; index++)
                if(lootToPick[index] == loot)
                {
                    lootToPick.RemoveAt(index);
                    Destroy(loot);
                }
            gatheredLoot++;
            lootPickupTime = Time.time;
            if(gatheredLoot == capacity)
            {
                returnToParty = true;
                gatherLoot = false;
            }
        }

        #region Loot Pre-Processing

        private void FindLootToPick()
        {
            lootToPick = GameObject.FindGameObjectsWithTag("Loot").ToList<GameObject>();

            Vector3 worldTouch = Camera.main.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, Camera.main.nearClipPlane + 5.0f));

            for (int index = 0; index < lootToPick.Count; index++)
            {
                if (worldTouch.x < 0 && lootToPick[index].transform.position.x > 0)
                {
                    lootToPick.RemoveAt(index);
                    index--;
                }
                else if (worldTouch.x > 0 && lootToPick[index].transform.position.x < 0)
                {
                    lootToPick.RemoveAt(index);
                    index--;
                }
            }
        }

        public double CalculateDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Pow((a.x - b.x), 2) + Mathf.Pow((a.y - b.y), 2) + Mathf.Pow((a.z - b.z), 2);
        }

        private void SortLoot()
        {
            List<GameObject> unvisitedNodes = new List<GameObject>(lootToPick);  //The equipment that we need to visit
            List<GameObject> visitedNodes = new List<GameObject>(); //Will hold the equipment in the order that we have visited them

            Vector3 nextPos = Vector3.zero;
            Vector3 currentPos = transform.position;

            int minDistIndex = 0;
            double minDist;
            double currentDistance;
            int count = unvisitedNodes.Count;                       //O(n)

            //For each node that hasn't been visited
            while (count != 0)                                      // O(n)
            {
                //We check the distance from this point to each equipment that hasn't been visited yet
                minDist = double.MaxValue;
                for (int index = 0; index < count; index++)         //O(n)
                {
                    currentDistance = CalculateDistance(currentPos, unvisitedNodes[index].transform.position);
                    if (currentDistance < minDist)
                    {
                        minDist = currentDistance;
                        minDistIndex = index;
                        nextPos = unvisitedNodes[index].transform.position;
                    }
                }
                currentPos = nextPos;
                visitedNodes.Add(unvisitedNodes[minDistIndex].gameObject);
                unvisitedNodes.RemoveAt(minDistIndex);              //O(logn)
                count = unvisitedNodes.Count;                       //O(logn)
            }
            lootToPick = visitedNodes;
        }

        #endregion

        #region InheritedMethods

        public override void Attack()
        {
            FindLootToPick();
            if (lootToPick.Count != 0)
            {
                SortLoot();
                gatherLoot = true;
                CharacterChanger.instance.ChangeSelectedCharacter();
            }
        }

        public override void ChangeBasePosition(Vector3 position)
        {
            if(gatherLoot || returnToParty)
            {
                returnPos = position;
                if (isSelected)
                    CharacterChanger.instance.ChangeSelectedCharacter();
            }
            else
            {
                base.ChangeBasePosition(position);
            }
        }

        public override void RotateTowardsTouchPos()
        {
            base.RotateTowardsTouchPos();
        }

        #endregion
    }
}
