using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class UIEndMissionManager : MonoBehaviour
    {
        public GameObject gameWonCanvas;
        public GameObject gameOverCanvas;
        public GameObject gamePausedCanvas;

        public GameObject[] stars;
        public GameObject[] starsBackground;

        private void Start()
        {
            gameWonCanvas.SetActive(false);
            gameOverCanvas.SetActive(false);
            gamePausedCanvas.SetActive(false);

            for (int index = 0; index < stars.Length; index++)
            {
                stars[index].SetActive(false);
                starsBackground[index].SetActive(false);
            }
        }

        public void DisplayGameWon(int earnedStars)
        {
            gameWonCanvas.SetActive(true);

            for(int index = 0; index < stars.Length; index++)
            {
                if (index < earnedStars)
                    stars[index].SetActive(true);
                else
                    starsBackground[index].SetActive(true);
            }
        }

        public void DisplayGameOver()
        {
            gameOverCanvas.SetActive(true);
        }

        public void DisplayPauseMenu(bool value)
        {
            gamePausedCanvas.SetActive(value);
        }
    }
}