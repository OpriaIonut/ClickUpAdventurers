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

        public void AwakeInit()
        {
            if (instance != null)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        #endregion

        public QuestScriptableObj selectedQuest;

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void SaveQuest(QuestScriptableObj quest)
        {
            selectedQuest = quest;
        }
    }
}