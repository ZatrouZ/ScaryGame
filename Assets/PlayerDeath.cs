using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class PlayerDeath : MonoBehaviour
{
    // Animator component (optional, if using death animation)
    public Animator animator;

    // Boolean to use a Game Over screen (optional)
    public bool useGameOverScreen = false;

    // Optional Game Over Manager reference (for showing Game Over UI)
    // public GameOverManager gameOverManager;

    // This method is called when a collision occurs
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player object (tagged as "Player") collides with an enemy (tagged as "Enemy")
        if (gameObject.CompareTag("Player") && collision.gameObject.CompareTag("Enemy"))
        {
            Die(); // Trigger player death logic
        }
    }

    // Handle player's death
    void Die()
    {
        Debug.Log("Player died!");

        // Option 1: Play death animation (if using an Animator)
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Option 2: Show Game Over screen (optional, if using a Game Over Manager)
        if (useGameOverScreen)
        {
            // Uncomment this line if you have a GameOverManager and Game Over logic
            // gameOverManager.ShowGameOverScreen();

            Debug.Log("Game Over screen should be shown!");
        }
        else
        {
            // Option 3: Reload the scene (restart the level)
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Option 4: Destroy the player GameObject (optional)
        // Destroy(gameObject);
    }
}