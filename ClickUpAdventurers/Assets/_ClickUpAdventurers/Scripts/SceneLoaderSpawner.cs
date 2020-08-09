using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClickUpAdventurers
{
    public class SceneLoaderSpawner : MonoBehaviour
    {
        private void Start()
        {
            //If we don't have a scene loader
            if (SceneLoader.instance == null)
            {
                //Transform this object into a scene loader
                gameObject.AddComponent(typeof(SceneLoader));
                gameObject.name = "SceneLoader";
                Destroy(this);
            }
        }
    }
}