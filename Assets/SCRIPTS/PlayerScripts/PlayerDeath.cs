using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class PlayerDeath : MonoBehaviour
{
    // Reference to the player's Animator (if using a death animation for the player)
    public Animator playerAnimator;

    // Boolean to use a Game Over screen (optional)
    public bool useGameOverScreen = false;

    // Optional Game Over Manager reference (for showing Game Over UI)
    // public GameOverManager gameOverManager;

    // Handle jumpscare: Reference to the monster's Animator
    public Animator monsterAnimator; // Assign this in the Inspector

    // This method is called when a trigger occurs
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player object (tagged as "Player") collides with an enemy (tagged as "Enemy")
        if (CompareTag("Player") && other.CompareTag("Enemy"))
        {
            TriggerJumpscare(other.gameObject);
            Die();
            Debug.Log("Player died.");
        }
    }

    // Trigger jumpscare animation on the enemy (monster)
    void TriggerJumpscare(GameObject enemy)
    {
        // Check if the monster has an Animator, and trigger the Jumpscare animation
        if (monsterAnimator != null)
        {
            monsterAnimator.SetTrigger("Jumpscare"); // This will play the monster's jumpscare animation
        }
    }

    // Handle player's death
    void Die()
    {
        Debug.Log("Player died!");

        // Option 1: Play death animation for the player (if using an Animator)
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Death");
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Option 4: Destroy the player GameObject (optional)
        // Destroy(gameObject);
    }
}