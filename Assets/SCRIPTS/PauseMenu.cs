using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; // Static variable to track the game's pause state
    public GameObject pauseMenuUI; // Reference to the pause menu UI
    private AudioSource[] audioSources; // Array to hold all audio sources

    void Start()
    {
        // Get all AudioSources in the scene to pause/resume them
        audioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        // Listen for the Escape key to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume(); // Resume the game if it's paused
            }
            else
            {
                Pause(); // Pause the game if it's not paused
            }
        }
    }

    // Resume the game and hide the pause menu
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Set the time scale back to normal
        GameIsPaused = false; // Update the pause state

        // Resume audio
        SetAudioSourcesState(true);

        // Hide the cursor and lock it for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Pause the game and show the pause menu
    void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Stop the game time
        GameIsPaused = true; // Update the pause state

        // Pause audio
        SetAudioSourcesState(false);

        // Unlock and show the cursor for the pause menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Load the main menu scene
    public void LoadMenu()
    {
        Time.timeScale = 1f; // Ensure the time scale is normal when loading a new scene
        SceneManager.LoadScene(0); // Load the scene with index 0 (main menu)
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // This only works in a built game, not in the editor
    }

    // Set the active state of all AudioSources
    // Set the active state of all AudioSources
    private void SetAudioSourcesState(bool isPlaying)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != null) // Ensure the audioSource is still valid
            {
                if (isPlaying)
                {
                    audioSource.UnPause(); // Resume audio playback
                }
                else
                {
                    audioSource.Pause(); // Pause audio playback
                }
            }
        }
    }

}
