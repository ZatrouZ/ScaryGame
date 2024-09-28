using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Head Bob Settings")]
    public float walkBobSpeed = 5f;     // Speed of the bob when walking
    public float runBobSpeed = 8f;      // Speed of the bob when running
    public float idleBobSpeed = 1f;     // Speed of the bob when idle
    public float bobAmount = 0.05f;     // Amount of head bob (amplitude)

    private float defaultYPos = 0f;     // Default Y position of the camera
    private float timer = 0f;           // Timer for sine wave calculation
    private CharacterController controller;  // Reference to the player's movement controller

    [Header("Player Movement Settings")]
    public float walkSpeedThreshold = 2f; // Threshold speed for walking
    public float runSpeedThreshold = 6f;  // Threshold speed for running

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();  // Get the CharacterController on the player
        defaultYPos = transform.localPosition.y;  // Store the default Y position of the camera
    }

    void Update()
    {
        HeadBob();
    }

    void HeadBob()
    {
        // Get the current speed of the player
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (speed > runSpeedThreshold) // Running state
        {
            // Faster bob speed when running
            timer += Time.deltaTime * runBobSpeed;
        }
        else if (speed > walkSpeedThreshold) // Walking state
        {
            // Medium bob speed when walking
            timer += Time.deltaTime * walkBobSpeed;
        }
        else // Idle state
        {
            // Slower bob speed when idle
            timer += Time.deltaTime * idleBobSpeed;
        }

        // Calculate the new Y position of the camera using a sine wave for smooth head bobbing
        float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;

        // Apply the new Y position to the camera
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}
