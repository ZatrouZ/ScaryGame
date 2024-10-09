using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightBob : MonoBehaviour
{
    [Header("Bob Settings")]
    public float idleBobSpeed = 0.5f;          // Bob speed when idle
    public float walkBobSpeed = 1.5f;          // Bob speed when walking
    public float runBobSpeed = 2.5f;           // Bob speed when running
    public float bobAmount = 0.02f;            // Amount of bobbing movement (position)
    public float rotationBobAmount = 1.5f;     // Amount of bobbing movement (rotation)

    [Header("Movement Settings")]
    public float walkSpeedThreshold = 1.5f;    // Threshold speed for walking
    public float runSpeedThreshold = 5f;       // Threshold speed for running

    private Vector3 initialPosition;           // Original local position of the flashlight
    private Quaternion initialRotation;        // Original local rotation of the flashlight
    private float bobTimer = 0f;               // Timer to drive the bobbing movement

    private Rigidbody playerRigidbody;         // Reference to player's Rigidbody

    void Start()
    {
        // Store the initial local position and rotation of the flashlight
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Get the Rigidbody from the parent object (usually the player)
        playerRigidbody = GetComponentInParent<Rigidbody>();

        if (playerRigidbody == null)
        {
            Debug.LogError("Player Rigidbody not found! Make sure the flashlight is a child of the player.");
        }
    }

    void Update()
    {
        HandleFlashlightBob();
    }

    void HandleFlashlightBob()
    {
        if (playerRigidbody == null) return;

        // Get the player's horizontal movement speed (ignoring vertical velocity)
        float speed = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z).magnitude;

        // Determine the bob speed based on whether the player is idle, walking, or running
        float bobSpeed;
        if (speed > runSpeedThreshold)
        {
            bobSpeed = runBobSpeed;   // Running
        }
        else if (speed > walkSpeedThreshold)
        {
            bobSpeed = walkBobSpeed;  // Walking
        }
        else
        {
            bobSpeed = idleBobSpeed;  // Idle
        }

        // Increase the bob timer using the selected speed
        bobTimer += Time.deltaTime * bobSpeed;
    }

    // Get the current position bobbing offset based on the bobTimer
    public Vector3 GetBobbingOffset()
    {
        float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
        float bobOffsetX = Mathf.Sin(bobTimer * 2) * bobAmount * 0.5f;  // Adds a bit of side-to-side bobbing
        return new Vector3(bobOffsetX, bobOffsetY, 0);
    }

    // Get the current rotation bobbing offset based on the bobTimer
    public Quaternion GetBobbingRotation()
    {
        float tiltRotationZ = Mathf.Cos(bobTimer) * rotationBobAmount;
        return Quaternion.Euler(0, 0, tiltRotationZ);
    }
}

