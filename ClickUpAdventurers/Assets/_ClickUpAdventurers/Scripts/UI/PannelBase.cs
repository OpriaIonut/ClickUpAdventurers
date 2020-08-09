using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClickUpAdventurers
{
    //Class used to make swapping between panels easier
    public class PannelBase : MonoBehaviour
    {
        public PannelBase previousPanel;    //The panel that we activate when we press the back/close button
        public PannelBase nextPanel;        //The panel that will be activated once we select something in the current panel

        //Disable the current panel and enable the next one
        public void ActivateNext()
        {
            DeactivateCurrent();
            nextPanel.ActivateCurrent();
            //Quest manager needs to know which panel is the currently selected one
            QuestUIManager.instance.ChangeSelected(nextPanel);  
        }

        //Disable the current panel and activate the previous one
        public void ActivatePrevious()
        {
            if (previousPanel == null)
            {
                //If we don't have one, then load the main building
                //The concept was made for the quest scene, in other scenes you may need to modify the code
                SceneLoader.instance.LoadScene("MainBuilding");
            }
            else
            {
                //If we have a previous panel then activate it
                DeactivateCurrent();
                previousPanel.ActivateCurrent();
                QuestUIManager.instance.ChangeSelected(previousPanel);
            }
        }

        //These methods can be overwritten to suit the script's behaviour
        public virtual void ActivateCurrent()
        {
            gameObject.SetActive(true);
        }

        public virtual void DeactivateCurrent()
        {
            gameObject.SetActive(false);
        }
    }
}