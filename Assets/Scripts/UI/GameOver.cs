using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public DistanceScore distanceScore;
    public GameObject scoreText;

    public void ShowGameOver()
    {
        distanceScore.StopScoring();

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        scoreText.SetActive(false);

        ShowFinalScore();
        ShowHighScores();
    }

    void ShowFinalScore()
    {
        finalScoreText.text = "Score: " + distanceScore.score;
    }

    void ShowHighScores()
    {
        highScoreText.text = "";
        float[] scores = distanceScore.highScoreData.scores;

        for (int i = 0; i < scores.Length; i++)
        {
            string rank = GetRankName(i);
            highScoreText.text += string.Format("{0,-8} {1,3}\n", rank, (int)scores[i]);
        }
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

    string GetRankName(int index)
    {
        switch (index)
        {
            case 0: return "1st";
            case 1: return "2nd";
            case 2: return "3rd";
            case 3: return "4th";
            case 4: return "5th";
            default: return (index + 1) + "th";
        }
    }
}
