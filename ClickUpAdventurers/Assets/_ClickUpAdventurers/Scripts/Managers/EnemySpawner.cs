using UnityEngine;

namespace ClickUpAdventurers
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform playerPos;

        public GameObject enemyToSpawn;
        public float timeBetweenEnemies = 2.0f;

        private void Start()
        {
            InvokeRepeating("SpawnEnemy", 0, timeBetweenEnemies);
        }

        private void SpawnEnemy()
        {
            bool spawnRight = Random.Range(0, 2) == 0 ? true : false;

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