using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public enum QuestRank
    {
        None,
        D,
        C,
        B,
        A,
        S
    };

    [CreateAssetMenu(fileName = "QuestTemplate", menuName = "ScriptableObjects/QuestScriptable")]
    public class QuestScriptableObj : ScriptableObject
    {
        public QuestRank questRank;
        public int rewardMoney;
        public List<WaveScriptableObj> waves;

        public string title;
        [TextArea(10, 20)]
        public string description;
    }
}