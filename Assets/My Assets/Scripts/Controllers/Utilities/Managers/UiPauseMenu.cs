using UnityEngine;
using UnityEngine.SceneManagement;

public class UiPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    public void PauseGame()
    {
        if (_pauseMenu.activeSelf)
        {
            ResumeGame(); return;
        }
        _pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Ensure Time.timeScale is reset to 1 when restarting
        Time.timeScale = 1;
        // Reload the current scene using its build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        // Ensure Time.timeScale is reset to 1 when loading the main menu
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}