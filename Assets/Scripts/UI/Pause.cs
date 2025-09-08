using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    public DungeonGenerator dungeonGenerator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void RestartGame()
    {
        dungeonGenerator.StartGame();
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        if (GameSession.instance != null) Destroy(GameSession.instance.gameObject);
        AudioManager.Instance.musicSrc.Stop();
        SceneManager.LoadScene(0);
    }
}
