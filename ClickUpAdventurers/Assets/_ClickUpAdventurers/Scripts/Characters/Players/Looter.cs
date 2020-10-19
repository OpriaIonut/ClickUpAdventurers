using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class Looter : PlayerCharacter
    {
        public float gatherTime = 1.0f; //The time it takes to gather 1 loot
        public int capacity = 5;        //The maximum capacity, once reached he will return to the party
        public float movementSpeed = 5;
        public int health = 3;

        public Image healthbar;

        private int gatheredLoot = 0;   //The loot he gathered in the current trip
        [HideInInspector] public bool gatherLoot = false;
        [HideInInspector] public bool returnToParty = false;

        private float lootPickupTime = 0;   //The time he reached the pickup
        private List<GameObject> lootToPick;    //List of the items to pick, sorted with Dijkstra by distance
        private Vector3 returnPos;  //The position to return to, will be set by the CharacterChanger
        private Quaternion initRot;

        private int healthInitVal;
        private float healthbarInitVal;

        private void Start()
        {
            base.InheritedStartCalls();

            initRot = transform.rotation;

            healthInitVal = health;
            healthbarInitVal = healthbar.rectTransform.localScale.x;

            healthbar.transform.parent.gameObject.SetActive(false);
        }

        private bool resetValuesPause = false;
        private void Update()
        {
            base.InheritedUpdateCalls();
            if (!BattleManager.instance.GamePaused)
            {
                if(resetValuesPause)
                {
                    lootPickupTime += BattleManager.instance.PausedTimeDiff;
                    resetValuesPause = false;
                }

                if (Time.time - lootPickupTime > gatherTime)
                {
                    GatherLoot();
                }
            }
            else
            {
                resetValuesPause = true;
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
                //If we still have capacity and want to gather loot
                //Check to see if the next loot is correct (it may have been destroyed)
                while (lootToPick.Count != 0 && lootToPick[0] == null)
                    lootToPick.RemoveAt(0);

                if (lootToPick.Count == 0)
                {
                    //If we don't have any loot return to the party
                    returnToParty = true;
                    gatherLoot = false;
                }
                else
                {
                    //Otherwise move towards the loot
                    Vector3 moveTowards = Vector3.MoveTowards(transform.position, lootToPick[0].transform.position, Time.deltaTime * movementSpeed);
                    transform.position = moveTowards;

                    Vector3 lookPos = lootToPick[0].transform.position - transform.position;
                    lookPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(lookPos);
                }
            }
            if (returnToParty)
            {
                //If we don't have any loot left to pick or have reached maximum capacity, return to the party
                Vector3 moveTowards = Vector3.MoveTowards(transform.position, returnPos, Time.deltaTime * movementSpeed);
                transform.position = moveTowards;
                if (transform.position == returnPos)
                {
                    //If we reached the destination then end the search (reset everything needed)
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
            //Add the loot to the stash and reset variables
            BattleManager.instance.GatheredLoot += gatheredLoot;
            gatheredLoot = 0;
            returnToParty = false;
            transform.rotation = initRot;
        }

        public void TakeDamage(int value)
        {
            if (!BattleManager.instance.GamePaused)
            {
                healthbar.transform.parent.gameObject.SetActive(true);
                health -= value;
                if (health <= 0)
                {
                    health = 0;
                    gameObject.SetActive(false);
                }
                healthbar.rectTransform.localScale = new Vector3(1 - healthbarInitVal * (healthInitVal - health) / healthInitVal, 1, 1);
            }
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
            //Find all loot
            lootToPick = GameObject.FindGameObjectsWithTag("Loot").ToList<GameObject>();

            Vector3 worldTouch = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane + 5.0f));

            for (int index = 0; index < lootToPick.Count; index++)
            {
                //Remove the loot that is not on the same side of the map that we are searching on
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

        //Sort the loot based on distance using Dijkstra
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

        public override void ChangeBasePosition(Vector3 position, Quaternion rot)
        {
            //If we are away from the party, change the return position
            if(gatherLoot || returnToParty)
            {
                returnPos = position;
                if (isSelected)
                    CharacterChanger.instance.ChangeSelectedCharacter();
            }
            else
            {
                //Otherwise change the position
                base.ChangeBasePosition(position, rot);
            }
        }

        public override void RotateTowardsTouchPos()
        {
            base.RotateTowardsTouchPos();
        }

        #endregion
    }
}
