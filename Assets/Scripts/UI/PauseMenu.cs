using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuUI;

    public void OnPauseClick()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnHomeClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnResumeClick()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }

    public void OnRestartClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
