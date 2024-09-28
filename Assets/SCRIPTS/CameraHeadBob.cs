using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Head Bob Settings")]
    public float walkBobSpeed = 3f;        // Slightly slower bob speed when walking
    public float runBobSpeed = 6f;         // Faster bob speed when running
    public float idleBobSpeed = 0.5f;      // Minimal bobbing when idle
    public float walkBobAmount = 0.02f;    // Subtle bobbing when walking
    public float runBobAmount = 0.04f;     // More exaggerated bobbing when running
    public float idleBobAmount = 0.01f;    // Very minimal bobbing when idle
    public float tiltAmount = 2f;          // Head tilt angle during movement

    private float defaultYPos;             // Default Y position of the camera
    private float defaultXPos;             // Default X position of the camera
    private float defaultZRot;             // Default rotation (Z axis) of the camera
    private float timer = 0f;              // Timer for sine wave calculation
    private Rigidbody playerRigidbody;     // Reference to the player's Rigidbody

    [Header("Player Movement Settings")]
    public float walkSpeedThreshold = 1.5f;  // Speed threshold for walking
    public float runSpeedThreshold = 5f;     // Speed threshold for running

    private bool isMoving = false;         // Tracks if the player is moving
    private bool isRunning = false;        // Tracks if the player is running

    void Start()
    {
        // Get the Rigidbody from the parent object (player)
        playerRigidbody = GetComponentInParent<Rigidbody>();

        // Store the default positions and rotation of the camera
        defaultYPos = transform.localPosition.y;
        defaultXPos = transform.localPosition.x;
        defaultZRot = transform.localEulerAngles.z;
    }

    void Update()
    {
        HeadBob();
    }

    void HeadBob()
    {
        // Get the player's speed, ignoring vertical movement
        float speed = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z).magnitude;

        isMoving = speed > 0.1f;
        isRunning = speed > runSpeedThreshold;

        if (isMoving)
        {
            // Adjust bob speed based on whether running or walking
            if (isRunning)
            {
                timer += Time.deltaTime * runBobSpeed;  // Running
            }
            else
            {
                timer += Time.deltaTime * walkBobSpeed;  // Walking
            }
        }
        else
        {
            timer += Time.deltaTime * idleBobSpeed;  // Idle
        }

        // Bob amount changes based on movement state
        float bobAmount = isRunning ? runBobAmount : (isMoving ? walkBobAmount : idleBobAmount);

        // Vertical bobbing with slight tilt
        float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;
        float newX = defaultXPos + Mathf.Cos(timer * 2f) * (bobAmount / 2f); // Phase shift for X-axis sway

        // Slight camera tilt (Z rotation) to simulate head rotation
        float newTilt = defaultZRot + Mathf.Sin(timer) * tiltAmount;

        // Apply smooth bobbing to the camera's position and tilt
        transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newTilt);
    }
}
