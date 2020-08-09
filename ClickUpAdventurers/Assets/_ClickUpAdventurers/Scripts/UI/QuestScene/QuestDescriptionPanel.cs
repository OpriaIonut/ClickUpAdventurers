using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class QuestDescriptionPanel : PannelBase
    {
        [Tooltip("Prefab for the image/button that you instantiate, that describes what enemies are in a quest.")]
        public GameObject questEnemyPrefab;

        public TextMeshProUGUI title;
        public TextMeshProUGUI rank;
        public TextMeshProUGUI description;
        public TextMeshProUGUI reward;

        public Transform enemyList;

        //Remeber the ui images so you delete them when deactivating the panel
        private List<GameObject> enemyUIInstances;  

        public void ShowExtraInfo(QuestScriptableObj quest)
        {
            title.text = "Title: " + quest.title;
            rank.text = "Rank: " + quest.questRank.ToString();
            description.text = quest.description;
            reward.text = "Reward: " + quest.rewardMoney;

            List<int> counts = new List<int>();
            enemyUIInstances = new List<GameObject>();
            List<GameObject> countedEnemies = new List<GameObject>();
            foreach(WaveScriptableObj waves in quest.waves)
            {
                foreach(WavePair enemy in waves.enemies)
                {
                    //If we don't have an enemy in the list
                    if(!countedEnemies.Contains(enemy.enemy))
                    {
                        //Then add him to the lists
                        enemyUIInstances.Add(Instantiate(questEnemyPrefab, enemyList));
                        countedEnemies.Add(enemy.enemy);
                        counts.Add(enemy.count);
                    }
                    else
                    {
                        //Otherwise, just increase it's count
                        int index = countedEnemies.FindIndex(obj => obj == enemy.enemy);
                        counts[index] += countedEnemies.Count;
                    }
                }
            }
            //Display the elements
            for(int index = 0; index < enemyUIInstances.Count; index++)
            {
                enemyUIInstances[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = countedEnemies[index].name;
                enemyUIInstances[index].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "x" + counts[index];
            }

            //Remember the current quest. In case we pick this one, he will be saved to the SceneLoader
            QuestUIManager.instance.SaveQuest(quest);
        }

        public override void DeactivateCurrent()
        {
            base.DeactivateCurrent();

            //Destroy the ui instances
            for (int index = 0; index < enemyUIInstances.Count; index++)
                Destroy(enemyUIInstances[index]);
            enemyUIInstances.Clear();
        }
    }
}