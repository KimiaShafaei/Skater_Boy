using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        highScoreText.text = "High Score: " + highScore;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
