using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform playerPos;
        public float timeBetweenEnemies = 2.0f;
        public float timeBetweenWaves = 10.0f;

        private QuestScriptableObj selectedQuest;   //The quest that the player has selected from the guild

        private bool checkForEndGame = false;
        private Looter looter;
        private Warrior[] warriors;

        private void Start()
        {
            looter = FindObjectOfType<Looter>();
            selectedQuest = SceneLoader.instance.selectedQuest;

            warriors = FindObjectsOfType<Warrior>();
            StartCoroutine(SpawnWaves());
        }

        private void Update()
        {
            if (checkForEndGame && CheckEndGame())
            {
                checkForEndGame = false;
                BattleManager.instance.GameWon();
            }
        }

        private bool CheckEndGame()
        {
            EnemyCharacter[] enemy = FindObjectsOfType<EnemyCharacter>();
            Loot[] loot = FindObjectsOfType<Loot>();

            if(enemy.Length == 0)
            {
                if(looter != null)
                {
                    //If there is no more loot and the looter is back with the party, then end the game
                    if (loot.Length == 0 && looter.gatherLoot == false && looter.returnToParty == false)
                        return true;
                    return false;
                }
                else
                {
                    //Looter is dead so no need to wait for loot to dissapear
                    return false;
                }
            }
            return false;
        }

        private IEnumerator SpawnWaves()
        {
            List<GameObject> waveEnemies;   //List containing all the enemies in a wave. Used when spawning randomly
            foreach(WaveScriptableObj wave in selectedQuest.waves)
            {
                waveEnemies = new List<GameObject>();
                //For all enemies in a wave
                foreach (WavePair category in wave.enemies)
                {
                    for (int index = 0; index < category.count; index++)
                    {
                        //If we spawn in order, simply spawn the enemy
                        if (wave.spawnOrder == WaveInstantiatingOrder.InOrder)
                        {
                            SpawnEnemy(category.enemy);
                            yield return new WaitForSeconds(timeBetweenEnemies);
                        }
                        else if (wave.spawnOrder == WaveInstantiatingOrder.Random)
                        {
                            //Otherwise add him to a list so we can spawn him randomly
                            waveEnemies.Add(category.enemy);
                        }
                    }
                }
                if(wave.spawnOrder == WaveInstantiatingOrder.Random)
                {
                    //While we still have enemies to spawn
                    while(waveEnemies.Count != 0)
                    {
                        //Spawn them randomly
                        int index = Random.Range(0, waveEnemies.Count);
                        SpawnEnemy(waveEnemies[index]);
                        waveEnemies.RemoveAt(index);
                        yield return new WaitForSeconds(timeBetweenEnemies);
                    }
                }

                RecoverWarriorHp();
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            checkForEndGame = true;
        }

        private void RecoverWarriorHp()
        {
            for (int index = 0; index < warriors.Length; index++)
                if (warriors[index] != null)
                    warriors[index].Health += warriors[index].endWaveRecoveryAmmount;
        }

        private void SpawnEnemy(GameObject enemyToSpawn)
        {
            if (!BattleManager.instance.GamePaused)
            {
                //Spawn the enemy to the left of the screen or to the right?
                bool spawnRight = Random.Range(0, 2) == 0 ? true : false;

                //Spawn him juuust outside the screen bounds
                Vector3 spawnPosition = new Vector3(1.1f, Random.Range(0.0f, 1.0f), Camera.main.transform.position.y);
                if (!spawnRight)
                    spawnPosition.x = -0.1f;
                Vector3 targetPos = Camera.main.ViewportToWorldPoint(spawnPosition);
                targetPos.y = playerPos.position.y;

                Transform clone = Instantiate(enemyToSpawn, transform).transform;
                clone.position = targetPos;
                clone.GetComponent<EnemyCharacter>().SetTarget(playerPos.position);
            }
        }
    }
}