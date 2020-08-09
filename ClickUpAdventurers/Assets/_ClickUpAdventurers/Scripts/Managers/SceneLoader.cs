using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClickUpAdventurers
{
    public class SceneLoader : MonoBehaviour
    {
        #region Singleton

        public static SceneLoader instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        #endregion

        public QuestScriptableObj selectedQuest;

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void SaveData(QuestScriptableObj quest)
        {
            selectedQuest = quest;
        }
    }
}