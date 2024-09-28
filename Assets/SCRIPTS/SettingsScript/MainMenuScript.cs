using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1); // Load the scene with index 1 (your game scene)
    }

    public void FullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen; // Toggle fullscreen mode
        print("Changed screenmode");
    }

    public void QuitGame()
    {
        // Quit the game (this will only work in a built version, not in the Unity Editor)
        Application.Quit();
        print("Game is exiting...");

        // Optional: Add code here for special behavior (like saving game data, showing a "Thank You" screen, etc.)
    }
}

