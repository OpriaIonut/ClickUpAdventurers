using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClickUpAdventurers
{
    //Certain buttons need to load scenes, but we want all scene changing to happen through the scene loader
    //So at runtime, at start we add that method to the on click event
    public class ButtonLoadScene : MonoBehaviour
    {
        public string sceneToLoad;
        private void Start()
        {
            Button btn = GetComponent<Button>();
            btn.onClick.AddListener(() => SceneLoader.instance.LoadScene(sceneToLoad));
        }
    }
}