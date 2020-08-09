using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClickUpAdventurers
{
    public class QuestUIManager : MonoBehaviour
    {
        #region Singleton

        public static QuestUIManager instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this.gameObject);
            else
                instance = this;
        }

        #endregion

        public PannelBase startingPanel;

        //Used to retain the selected quest so that we don't write directly to the SceneLoader
        private QuestScriptableObj selectedQuest;   

        private void Start()
        {
            startingPanel.ActivateCurrent();
        }

        //Method called by button press
        public void SelectPrevious()
        {
            startingPanel.ActivatePrevious();
        }

        public void ChangeSelected(PannelBase elem)
        {
            startingPanel = elem;
        }

        public void SaveQuest(QuestScriptableObj quest)
        {
            selectedQuest = quest;
        }

        public void StartQuest()
        {
            SceneLoader.instance.SaveData(selectedQuest);   //Save the current quest so we can spawn the proper enemies
            SceneLoader.instance.LoadScene("BattleScene");
        }
    }
}