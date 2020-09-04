using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    public class QuestUISpawner : MonoBehaviour
    {
        public GameObject rankButtonPrefab;
        public GameObject questTitlePrefab;
        public List<QuestScriptableObj> quests;

        public GameObject canvasHeader;
        public GameObject canvasContent;

        public QuestListPanel questListScript;
        public QuestDescriptionPanel questDescriptionPanel;

        private void Start()
        {
            AddQuests();
        }

        private void AddQuests()
        {
            List<QuestRank> foundRanks = new List<QuestRank>(); //The ransk that we found in the list of quests
            questListScript.questsByRank = new List<List<GameObject>>();

            QuestRank minRank = QuestRank.S;    //initialization like double a = float.MaxValue bullshit
            foreach (QuestScriptableObj quest in quests)
            {
                //If we found a rank that we don't have in the list
                if (!foundRanks.Contains(quest.questRank))
                {
                    //Spawn a new tab for it and set the properties
                    GameObject clone = Instantiate(rankButtonPrefab, canvasHeader.transform);
                    TextMeshProUGUI text = clone.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                    text.text = quest.questRank.ToString();
                    foundRanks.Add(quest.questRank);
                    questListScript.questsByRank.Add(new List<GameObject>());

                    //If it is the minimum rank, then remember it
                    if (quest.questRank < minRank)
                        minRank = quest.questRank;

                    //Add listener for changing the rank of the quests
                    clone.GetComponent<Button>().onClick.AddListener(() => questListScript.SetCategory(quest.questRank));
                }
            }

            //For each quest
            for(int index = 0; index < quests.Count; index++)
            {
                //Get the id of the quest (used for ui component)
                int id = questListScript.questsByRank[(int)quests[index].questRank - 1].Count + 1;

                //Instantiate the ui button for the current quest and set it's properties
                Transform clone = Instantiate(questTitlePrefab, canvasContent.transform).GetComponent<Transform>();
                clone.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + id + ".";
                clone.GetChild(1).GetComponent<TextMeshProUGUI>().text = quests[index].title;
                clone.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + quests[index].rewardMoney + "$";

                Transform child3 = clone.GetChild(3);
                Transform child31 = clone.GetChild(3).GetChild(1);
                Image[] stars = child31.GetComponentsInChildren<Image>();

                int aquiredStars = DataRetainer.instance.GetQuestStars(quests[index].title);
                for (int starIndex = 0; starIndex < stars.Length; starIndex++)
                {
                    if(starIndex < aquiredStars)
                        stars[starIndex].gameObject.SetActive(true);
                    else
                        stars[starIndex].gameObject.SetActive(false);
                }

                //Add the quest to the list so we can enable/disable them when changing ranks
                questListScript.questsByRank[(int)quests[index].questRank - 1].Add(clone.gameObject);

                //Delegate takes parameters by referencce instead of value so we need a new copy of the variable
                int param = index;
                Button btn = clone.GetComponent<Button>();
                //Add button events
                btn.onClick.AddListener(() => questDescriptionPanel.ShowExtraInfo(quests[param]));
                btn.onClick.AddListener(() => questListScript.ActivateNext());
            }
            //Set the initial category to be the minimum rank one
            questListScript.SetCategory(minRank);
        }
    }
}