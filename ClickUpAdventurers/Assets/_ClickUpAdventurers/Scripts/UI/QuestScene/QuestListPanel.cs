using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    //Represents the panel that contains lists of all the available quests
    public class QuestListPanel : PannelBase
    {
        //List of list that contains the quests sorted by rank
        public List<List<GameObject>> questsByRank;

        //Activate only the buttons that have the rank that we chose
        public void SetCategory(QuestRank rank)
        {
            int category = (int)rank - 1;
            for(int index = 0; index < questsByRank.Count; index++)
            {
                for(int index2 = 0; index2 < questsByRank[index].Count; index2++)
                {
                    if(index != category)
                    {
                        questsByRank[index][index2].SetActive(false);
                    }
                    else
                    {
                        questsByRank[index][index2].SetActive(true);
                    }
                }
            }
        }
    }
}