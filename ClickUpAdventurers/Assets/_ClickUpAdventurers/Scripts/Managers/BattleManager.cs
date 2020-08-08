using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

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

    public GameObject endGameCanvas;
    public GameObject gamePausedCanvas;
    public int gatheredLoot = 0;

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

    //Used to know how much time has passed while the game was paused
    private float pausedTimeStart;
    private float pausedTimeEnd;

    private void Start()
    {
        endGameCanvas.SetActive(false);
    }

    private void Update()
    {
        if(gameHasEnded && Input.touchCount > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void EndGame()
    {
        gameHasEnded = true;
        endGameCanvas.SetActive(true);
    }

    public void TogglePause()
    {
        gamePaused = !gamePaused;

        if (gamePaused == true)
            pausedTimeStart = Time.time;
        else
            pausedTimeEnd = Time.time;

        gamePausedCanvas.SetActive(gamePaused);
    }
}
