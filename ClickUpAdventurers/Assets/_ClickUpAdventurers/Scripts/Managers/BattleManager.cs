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

    private bool gameHasEnded = false;
    public int gatheredLoot = 0;

    public bool GameHasEnded
    {
        get { return gameHasEnded; }
    }

    public GameObject endGameCanvas;

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
}
