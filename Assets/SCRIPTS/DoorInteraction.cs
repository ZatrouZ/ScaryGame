using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorPivot;             // Reference to the DoorPivot GameObject
    public float openAngle = 90f;           // Angle to open the door
    public float doorSpeed = 2f;            // Speed at which the door opens/closes
    public bool isLocked = false;           // State of the door (locked/unlocked)
    public int requiredKeyID;               // Key ID required to unlock the door

    private bool isOpen = false;            // State of the door
    private Quaternion closedRotation;      // Initial closed rotation of the door
    private Quaternion openRotation;        // Target open rotation of the door

    private Camera playerCamera;            // Reference to the player camera
    public float interactionDistance = 3f;  // Distance within which the player can interact with the door
    public float shakeAmount = 0.1f;        // Amount to shake the door
    public float shakeDuration = 0.5f;      // Duration of the shake effect

    public PlayerInventory playerInventory; // Reference to the player's inventory to check for keys

    [Header("UI Settings")]
    public Text doorInteractionText;        // UI Text that shows the door interaction message
    public float fadeDuration = 2f;         // Time taken for text to fade out
    public float displayDuration = 2f;      // Time before the text starts fading out

    [Header("Audio Settings")]
    public AudioSource audioSource;             // AudioSource component to play sounds
    public AudioClip lockedDoorSound;           // Sound clip for trying to open the locked door
    public AudioClip doorOpenSound;             // Sound clip for opening the door
    public AudioClip doorCloseSound;            // Sound clip for closing the door
    public AudioClip unlockWithKeySound;        // Sound clip for unlocking the door with a key

    private bool canShowText = true;        // To control when the text can be shown
    private float textCooldown = 5f;        // Cooldown time in seconds before text can be shown again

    void Start()
    {
        // Store the initial closed rotation of the door based on its pivot
        closedRotation = doorPivot.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;

        // Attempt to get the main camera
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not found! Ensure your camera is tagged as 'MainCamera'.");
        }

        // Ensure the interaction text is initially disabled
        if (doorInteractionText != null)
        {
            doorInteractionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is looking at the door and within interaction distance
        if (IsLookingAtDoor())
        {
            // Check for interaction input
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithDoor();
            }
        }

        // Smoothly rotate the door based on its current state
        if (isOpen)
        {
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, openRotation, Time.deltaTime * doorSpeed);
        }
        else
        {
            doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, closedRotation, Time.deltaTime * doorSpeed);
        }
    }

    void InteractWithDoor()
    {
        if (isLocked)
        {
            if (playerInventory.HasKey(requiredKeyID))  // Check if the player has the required key
            {
                Debug.Log("Door unlocked!");
                isLocked = false;  // Unlock the door
                PlayUnlockWithKeySound(); // Play unlock sound
                ToggleDoor();  // Open the door
            }
            else
            {
                Debug.Log("The door is locked. You need the correct key.");
                StartCoroutine(ShakeDoor()); // Shake the door to indicate it's locked
                PlayLockedDoorSound(); // Play sound for locked door interaction

                if (canShowText) // Only show the text if allowed (not on cooldown)
                {
                    ShowAndFadeInteractionText("The door is locked. You need the correct key."); // Show locked message
                    StartCoroutine(TextCooldown()); // Start cooldown before showing text again
                }
            }
        }
        else
        {
            ToggleDoor();  // Open/close the door if it's unlocked
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen; // Toggle the state (open or close the door)

        if (isOpen)
        {
            PlayDoorOpenSound(); // Play the door opening sound
        }
        else
        {
            PlayDoorCloseSound(); // Play the door closing sound
        }
    }

    // Coroutine to shake the door when locked
    IEnumerator ShakeDoor()
    {
        Vector3 originalPosition = doorPivot.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Calculate a random shake offset
            float xOffset = Random.Range(-shakeAmount, shakeAmount);
            float zOffset = Random.Range(-shakeAmount, shakeAmount);
            doorPivot.localPosition = originalPosition + new Vector3(xOffset, 0, zOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset door position back to original
        doorPivot.localPosition = originalPosition;
    }

    // Play the sound for interacting with a locked door
    void PlayLockedDoorSound()
    {
        if (audioSource != null && lockedDoorSound != null)
        {
            audioSource.PlayOneShot(lockedDoorSound); // Play the locked door sound
        }
    }

    // Play the sound for unlocking the door with a key
    void PlayUnlockWithKeySound()
    {
        if (audioSource != null && unlockWithKeySound != null)
        {
            audioSource.PlayOneShot(unlockWithKeySound); // Play the unlock sound
        }
    }

    // Play the sound for opening the door
    void PlayDoorOpenSound()
    {
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound); // Play the door opening sound
        }
    }

    // Play the sound for closing the door
    void PlayDoorCloseSound()
    {
        if (audioSource != null && doorCloseSound != null)
        {
            audioSource.PlayOneShot(doorCloseSound); // Play the door closing sound
        }
    }

    // Shows the interaction text and fades it out after a few seconds
    void ShowAndFadeInteractionText(string message)
    {
        if (doorInteractionText != null)
        {
            doorInteractionText.text = message; // Set the message
            doorInteractionText.gameObject.SetActive(true); // Enable the text

            // Start the coroutine to fade out the text after display
            StartCoroutine(FadeOutText());
        }
    }

    // Coroutine to fade out the interaction text after a delay
    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(displayDuration); // Wait before starting fade out

        float elapsedTime = 0f;
        Color originalColor = doorInteractionText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // Gradually reduce alpha
            doorInteractionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Once faded out, disable the text object
        doorInteractionText.gameObject.SetActive(false);
        doorInteractionText.color = originalColor; // Reset the color for the next use
    }

    // Cooldown to prevent text from being enabled again for a certain time
    IEnumerator TextCooldown()
    {
        canShowText = false; // Disable text showing
        yield return new WaitForSeconds(textCooldown); // Wait for the cooldown period
        canShowText = true; // Enable text showing again
    }

    bool IsLookingAtDoor()
    {
        if (playerCamera == null) return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform)
            {
                return true; // The player is looking at the door
            }
        }
        return false; // The player is not looking at the door
    }
}
