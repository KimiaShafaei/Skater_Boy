using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public DistanceScore distanceScore;
    public GameObject scoreText;
    public GameObject newRecordText;

    public TextMeshProUGUI[] rankTexts;
    public TextMeshProUGUI[] scoreTexts;

    public void ShowGameOver()
    {
        distanceScore.StopScoring();

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        scoreText.SetActive(false);

        ShowFinalScore();
        ShowHighScores();

        newRecordText.SetActive(distanceScore.isNewRecord);
    }

    void ShowFinalScore()
    {
        finalScoreText.text = "Score: " + distanceScore.score;
    }

    void ShowHighScores()
    {
        float[] scores = distanceScore.highScoreData.scores;

        for (int i = 0; i < scores.Length; i++)
        {
            rankTexts[i].text = GetRankName(i);
            scoreTexts[i].text = scores[i].ToString();
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
