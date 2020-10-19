using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace ClickUpAdventurers
{
    public class SceneLoaderSpawner : MonoBehaviour
    {
        public int warriorBaseHP = 100;
        public string saveFileName = "save_game0.json";
        public QuestScriptableObj[] allQuests;
        public ItemScriptableObj[] allItems;

        private void Awake()
        {
            DataSaver dataSaver = FindObjectOfType<DataSaver>();
            if(dataSaver == null)
            {
                SceneLoader sceneLoader = (SceneLoader)gameObject.AddComponent(typeof(SceneLoader));
                dataSaver = (DataSaver)gameObject.AddComponent(typeof(DataSaver));
                DataRetainer dataRetainer = (DataRetainer)gameObject.AddComponent(typeof(DataRetainer));
                EquipmentRetainer equipmentRetainer = (EquipmentRetainer)gameObject.AddComponent(typeof(EquipmentRetainer));

                dataSaver.fileName = saveFileName;

                dataRetainer.warriorBaseHP = warriorBaseHP;
                dataRetainer.allQuests = new QuestScriptableObj[allQuests.Length];
                for (int index = 0; index < allQuests.Length; index++)
                    dataRetainer.allQuests[index] = allQuests[index];

                equipmentRetainer.allItems = new ItemScriptableObj[allItems.Length];
                for (int index = 0; index < allItems.Length; index++)
                    equipmentRetainer.allItems[index] = allItems[index];

                sceneLoader.AwakeInit();
                dataSaver.AwakeInit(warriorBaseHP, allQuests.Length);
                equipmentRetainer.AwakeInit();
                dataRetainer.AwakeInit();

                gameObject.name = "SceneManagers";
                Destroy(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}