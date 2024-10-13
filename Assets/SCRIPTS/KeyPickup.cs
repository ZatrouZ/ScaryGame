using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    public int keyID;  // Unique key ID for this specific key
    public float interactionDistance = 3f;  // How close the player needs to be to interact with the key

    [Header("UI Settings")]
    public Text pickupText;  // UI Text element for displaying "Press E to pick up"

    private Camera playerCamera;
    private bool isLookingAtKey = false;  // Whether the player is looking at the key

    void Start()
    {
        playerCamera = Camera.main;  // Get the main camera

        // Hide the pickup text at the start
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);  // Make sure the text is hidden at the start
        }
    }

    void Update()
    {
        // Check if the player is looking at the key
        if (IsLookingAtKey())
        {
            isLookingAtKey = true;

            // Show "Press E to pick up" text when looking at the key
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(true);  // Show the pickup text
            }

            // Check if the player presses E while looking at the key
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickupKey();
            }
        }
        else
        {
            isLookingAtKey = false;

            // Hide the pickup text when not looking at the key
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);  // Hide the pickup text
            }
        }
    }

    // This method picks up the key
    void PickupKey()
    {
        PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddKey(keyID);  // Add the key to the player's inventory
            Destroy(gameObject);  // Destroy the key after pickup

            // Hide the pickup text after key is picked up
            if (pickupText != null)
            {
                pickupText.gameObject.SetActive(false);  // Hide text after picking up the key
            }
        }
    }

    // Check if the player is looking at the key and within interaction distance
    bool IsLookingAtKey()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.transform == transform)
            {
                return true;  // The player is looking at the key
            }
        }
        return false;  // The player is not looking at the key
    }
}
