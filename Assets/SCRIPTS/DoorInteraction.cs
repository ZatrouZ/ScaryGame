using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorPivot;             // Reference to the DoorPivot GameObject
    public float openAngle = 90f;           // Angle to open the door
    public float doorSpeed = 2f;            // Speed at which the door opens/closes
    public bool isLocked = false;            // State of the door (locked/unlocked)

    private bool isOpen = false;             // State of the door
    private Quaternion closedRotation;       // Initial closed rotation of the door
    private Quaternion openRotation;         // Target open rotation of the door

    private Camera playerCamera;             // Reference to the player camera
    public float interactionDistance = 3f;   // Distance within which the player can interact with the door
    public float shakeAmount = 0.1f;         // Amount to shake the door
    public float shakeDuration = 0.5f;       // Duration of the shake effect

    void Start()
    {
        // Store the initial closed rotation of the door based on its pivot
        closedRotation = doorPivot.localRotation; // Store the initial rotation of the pivot
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation; // Calculate open rotation based on closed rotation

        // Attempt to get the main camera
        playerCamera = Camera.main;

        // Check if playerCamera is null and output a warning
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not found! Ensure your camera is tagged as 'MainCamera'.");
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
                ToggleDoor();
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

    void ToggleDoor()
    {
        if (isLocked)
        {
            // Shake the door if it's locked
            StartCoroutine(ShakeDoor());
        }
        else
        {
            // Toggle the state if unlocked
            isOpen = !isOpen; // Toggle the state
        }
    }

    IEnumerator ShakeDoor()
    {
        Vector3 originalPosition = doorPivot.localPosition; // Store the original position of the door
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Calculate a random shake offset
            float xOffset = Random.Range(-shakeAmount, shakeAmount);
            float zOffset = Random.Range(-shakeAmount, shakeAmount);
            doorPivot.localPosition = originalPosition + new Vector3(xOffset, 0, zOffset); // Apply the shake offset

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset door position back to original
        doorPivot.localPosition = originalPosition;
    }

    bool IsLookingAtDoor()
    {
        // Create a ray from the camera
        if (playerCamera == null) return false; // Prevent null reference error

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if the raycast hit a GameObject with the "Door" tag
            if (hit.transform.CompareTag("Door"))
            {
                return true; // The player is looking at the door
            }
        }
        return false; // The player is not looking at the door
    }
}
