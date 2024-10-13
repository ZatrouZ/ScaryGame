using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorPivot;             // Reference to the DoorPivot GameObject
    public float openAngle = 90f;           // Angle to open the door
    public float doorSpeed = 2f;             // Speed at which the door opens/closes

    private bool isOpen = false;             // State of the door
    private Quaternion closedRotation;       // Initial closed rotation of the door
    private Quaternion openRotation;         // Target open rotation of the door

    private Camera playerCamera;             // Reference to the player camera
    public float interactionDistance = 3f;   // Distance within which the player can interact with the door

    void Start()
    {
        // Store the initial closed rotation of the door based on its pivot
        closedRotation = doorPivot.localRotation; // Store the initial rotation of the pivot
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation; // Calculate open rotation based on closed rotation
        playerCamera = Camera.main; // Get the main camera
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
        isOpen = !isOpen; // Toggle the state
    }

    bool IsLookingAtDoor()
    {
        // Create a ray from the camera
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
