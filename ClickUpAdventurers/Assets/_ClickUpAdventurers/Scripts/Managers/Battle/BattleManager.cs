using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace ClickUpAdventurers
{
    public class BattleManager : MonoBehaviour
    {
        #region Singleton

        public static BattleManager instance;

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }

        #endregion

        public TextMeshProUGUI lootText;
        public int lootMultiplier = 1;

        private int gatheredLoot = 0;
        public int GatheredLoot
        {
            get { return gatheredLoot; }
            set { gatheredLoot = value; lootText.text = "Loot: " + value; }
        }

        [Tooltip("A score will be calculated in a 0-100 range. If the score is higher than the x, y, z values, then a star will be added to the score for each x, y, z value")]
        public Vector3Int starsRequirement;

        #region Properties

        private bool gamePaused = false;
        public bool GamePaused
        {
            get { return gamePaused; }
        }

        private bool gameHasEnded = false;
        public bool GameHasEnded
        {
            get { return gameHasEnded; }
        }

        //Returns the time the game was paused
        public float PausedTimeDiff
        {
            get { return pausedTimeEnd - pausedTimeStart; }
        }

        #endregion

        private UIEndMissionManager uiManager;
        //Used to know how much time has passed while the game was paused
        private float pausedTimeStart;
        private float pausedTimeEnd;

        private void Start()
        {
            uiManager = FindObjectOfType<UIEndMissionManager>();

            Warrior[] warriors = FindObjectsOfType<Warrior>();
            DataRetainer dataRetainer = DataRetainer.instance;

            GatheredLoot = 0;
        }

        private void Update()
        {
            if(Input.touchCount > 0)
            {
                if(GameHasEnded)
                    SceneManager.LoadScene("MainScene");
                if (GamePaused)
                    TogglePause();
            }
        }

        public void GameWon()
        {
            Warrior[] warriors = FindObjectsOfType<Warrior>();
            int averageHealth = 0;
            foreach (Warrior item in warriors)
                averageHealth += item.Health;
            averageHealth /= warriors.Length; //Gives the average health in a 0-100 range

            int stars = 0;
            if (averageHealth >= starsRequirement.x)
                stars++;
            if (averageHealth >= starsRequirement.y)
                stars++;
            if (averageHealth >= starsRequirement.z)
                stars++;

            SaveData(stars, warriors, true);

            uiManager.DisplayGameWon(stars);
            gameHasEnded = true;
        }

        public void SaveData(int stars, Warrior[] warriors, bool wonMission)
        {
            DataRetainer dataRetainer = DataRetainer.instance;

            dataRetainer.Warrior1HP = 0;
            dataRetainer.Warrior2HP = 0;
            if (warriors.Length > 0)
                dataRetainer.Warrior1HP = warriors[0].Health;
            if (warriors.Length > 1)
                dataRetainer.Warrior2HP = warriors[1].Health;

            if(wonMission)
            {
                dataRetainer.Money += gatheredLoot * lootMultiplier;
                SceneLoader sceneLoader = SceneLoader.instance;
                int previousStars;
                if(dataRetainer.SaveQuestStars(sceneLoader.selectedQuest.title, stars, out previousStars))
                {
                    dataRetainer.Money += sceneLoader.selectedQuest.rewardMoney * (stars - previousStars);
                }
            }

            dataRetainer.SaveModifiedData();
        }

        public void GameOver()
        {
            gameHasEnded = true;
            uiManager.DisplayGameOver();
            Warrior[] warriors = FindObjectsOfType<Warrior>();
            SaveData(0, warriors, false);
        }

        public void TogglePause()
        {
            gamePaused = !gamePaused;

            if (gamePaused == true)
                pausedTimeStart = Time.time;
            else
                pausedTimeEnd = Time.time;

            uiManager.DisplayPauseMenu(gamePaused);
        }
    }
}