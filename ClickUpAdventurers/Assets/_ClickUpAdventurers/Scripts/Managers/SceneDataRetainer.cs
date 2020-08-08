using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class SceneDataRetainer : MonoBehaviour
    {
        #region Singleton

        public static SceneDataRetainer instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        #endregion

        //public int loot;
    }
}