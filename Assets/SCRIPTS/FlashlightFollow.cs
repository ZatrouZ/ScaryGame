using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    public Transform playerCamera;  // The camera that the flashlight will follow

    // Offset position relative to the camera, adjust this if necessary
    public Vector3 positionOffset = new Vector3(0.4f, -0.25f, 0.5f);

    // Optional offset for rotation if you want to adjust the flashlight's angle
    public Vector3 rotationOffset = new Vector3(80f, -2f, -4f);

    private Vector3 initialPosition;  // Initial local position of the flashlight
    private Quaternion initialRotation;  // Initial local rotation of the flashlight

    // References to the bobbing effect (from FlashlightBob)
    private FlashlightBob bobScript;

    void Start()
    {
        // Store initial local position and rotation for bobbing adjustments
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Get the FlashlightBob script if it exists on this object
        bobScript = GetComponent<FlashlightBob>();

        if (bobScript == null)
        {
            Debug.LogWarning("FlashlightBob script not found on the flashlight object.");
        }
    }

    void Update()
    {
        // Update flashlight's position relative to the camera
        Vector3 targetPosition = playerCamera.position + playerCamera.TransformDirection(positionOffset);

        // If the FlashlightBob script is enabled, add the bobbing effect to the position
        if (bobScript != null)
        {
            targetPosition += bobScript.GetBobbingOffset();
        }

        // Apply the new position
        transform.position = targetPosition;

        // Update flashlight's rotation to match the camera, with optional rotation offset
        Quaternion targetRotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);

        // If the FlashlightBob script is enabled, add the bobbing rotation
        if (bobScript != null)
        {
            targetRotation *= bobScript.GetBobbingRotation();
        }

        // Apply the new rotation
        transform.rotation = targetRotation;
    }
}
