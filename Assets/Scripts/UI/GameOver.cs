using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public DistanceScore distanceScore;
    public GameObject scoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        Debug.Log("high score: " + distanceScore.highScore);

        distanceScore.StopScoring();
        scoreText.SetActive(false);
        finalScoreText.text = "Final Score: " + distanceScore.score;
        highScoreText.text = "High Score: " + distanceScore.highScore;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WatchAd()
    {
        Debug.Log("Watch Ad");
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
