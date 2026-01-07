using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

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
