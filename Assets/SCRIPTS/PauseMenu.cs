using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        // Listen for the Escape key to toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();  // Resume the game if it's paused
            }
            else
            {
                Pause();  // Pause the game if it's not paused
            }
        }
    }

    // Resume the game and hide the pause menu
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Hide the cursor and lock it for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Pause the game and show the pause menu
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Unlock and show the cursor for the pause menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Load the main menu scene
    public void LoadMenu()
    {
        Time.timeScale = 1f;  // Ensure the time scale is normal when loading a new scene
        SceneManager.LoadScene(0);  // Load the scene with index 0 (main menu)
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();  // This only works in a built game, not in the editor
    }
}