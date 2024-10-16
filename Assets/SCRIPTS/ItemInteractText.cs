using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteractText : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;      // Distance within which the player can interact with the item
    private Camera playerCamera;                // Reference to the player camera
    public Text interactionText;                // Reference to the UI Text element

    private bool isWithinRange = false;         // Whether the player is within interaction range
    private bool isTextActive = false;          // Whether the text is currently active
    public float fadeDuration = 2f;             // Duration for text fade out

    void Start()
    {
        // Attempt to get the main camera
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not found! Ensure your camera is tagged as 'MainCamera'.");
        }

        // Ensure the interaction text is initially disabled
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is within interaction distance and looking at the item
        isWithinRange = IsLookingAtItem();

        // Automatically disable text if player is out of range
        if (!isWithinRange && isTextActive)
        {
            ToggleText(false); // Disable the text
        }

        // Check for interaction input (press E) when in range
        if (isWithinRange && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the text visibility
            ToggleText(true);
        }
    }

    // Toggle the interaction text on or off and start fade-out if turning on
    void ToggleText(bool active)
    {
        if (interactionText != null)
        {
            if (active)
            {
                interactionText.gameObject.SetActive(true); // Enable the text
                interactionText.color = new Color(interactionText.color.r, interactionText.color.g, interactionText.color.b, 1); // Reset to full opacity
                isTextActive = true;
                StartCoroutine(FadeOutText()); // Start fade out after 2 seconds
            }
            else
            {
                interactionText.gameObject.SetActive(false); // Disable the text
                isTextActive = false;
            }
        }
    }

    // Coroutine to fade out the interaction text over time
    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(2f); // Wait before starting fade out

        float elapsedTime = 0f;
        Color originalColor = interactionText.color;

        // Gradually reduce the alpha value to fade the text out
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // Lerp alpha from 1 to 0
            interactionText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // After fade-out, disable the text
        interactionText.gameObject.SetActive(false);
        isTextActive = false;
    }

    // Check if the player is looking at the item and within interaction distance
    bool IsLookingAtItem()
    {
        if (playerCamera == null) return false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform)
            {
                return true; // The player is looking at the item within range
            }
        }
        return false; // The player is not looking at the item
    }
}

