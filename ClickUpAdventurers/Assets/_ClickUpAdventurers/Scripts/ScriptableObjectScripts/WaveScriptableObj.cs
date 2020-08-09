using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    [Serializable]
    public struct WavePair
    {
        public GameObject enemy;    //The enemy to spawn in the wave
        public int count;           //The number of enemies of this type
    }

    //Are the enemies spawning in the order they appear in or randomly
    public enum WaveInstantiatingOrder
    {
        None,
        InOrder,
        Random
    }

    [CreateAssetMenu(fileName = "WaveTemplate", menuName = "ScriptableObjects/WaveScriptable")]
    public class WaveScriptableObj : ScriptableObject
    {
        public WaveInstantiatingOrder spawnOrder;

        [SerializeField]
        public List<WavePair> enemies;
    }
}