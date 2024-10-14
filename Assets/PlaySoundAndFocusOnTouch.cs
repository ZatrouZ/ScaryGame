using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAndFocusOnTouch : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip soundClip;              // Assign the sound clip in the Inspector
    private AudioSource audioSource;         // Reference to the AudioSource component

    [Header("Camera Focus Settings")]
    public Transform playerCamera;           // The player's camera
    public float focusSpeed = 2f;            // Speed at which the camera focuses on the object
    public float focusDuration = 5f;         // Duration to hold the camera focus
    public MonoBehaviour cameraBobScript;    // Reference to the Camera Bob script to deactivate

    [Header("Player Movement Settings")]
    public Rigidbody playerRigidbody;        // Player's Rigidbody for movement control
    public float playerSpeed = 5f;           // Speed of the player's movement

    private bool isFocusing = false;         // Is the camera currently focusing on the object?
    private Transform targetObject;          // The object to focus on

    void Start()
    {
        // Ensure there's an AudioSource component on the GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip; // Assign the sound clip to the AudioSource
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player")) // Ensure your player GameObject has the "Player" tag
        {
            // Play the sound when the player touches the object
            PlaySound();

            // Focus the camera on this object
            targetObject = transform; // Focus on the object this script is attached to
            StartCoroutine(FocusOnObject());
        }
    }

    void PlaySound()
    {
        // Play the sound effect if the sound clip is assigned
        if (soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
        else
        {
            Debug.LogWarning("Sound clip not assigned in " + gameObject.name);
        }
    }

    IEnumerator FocusOnObject()
    {
        // Disable the camera bob script during the focus
        if (cameraBobScript != null)
        {
            cameraBobScript.enabled = false;
        }

        isFocusing = true;
        float elapsedTime = 0f;

        while (elapsedTime < focusDuration)
        {
            elapsedTime += Time.deltaTime;

            // Smoothly rotate the camera to look at the target object
            Vector3 directionToTarget = (targetObject.position - playerCamera.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, Time.deltaTime * focusSpeed);

            // Adjust player movement to the new camera orientation
            AdjustPlayerMovement();

            // Check if the camera is very close to looking directly at the object
            float angle = Quaternion.Angle(playerCamera.rotation, targetRotation);
            if (angle < 0.5f)
            {
                break; // Stop focusing if close enough to the target rotation
            }

            yield return null;
        }

        yield return new WaitForSeconds(focusDuration); // Hold the focus for the specified time

        // Re-enable the camera bob script after focusing
        if (cameraBobScript != null)
        {
            cameraBobScript.enabled = true;
        }

        isFocusing = false;
    }

    void AdjustPlayerMovement()
    {
        // Calculate the player's movement direction based on the current camera orientation
        Vector3 forward = playerCamera.forward;
        forward.y = 0f;  // Prevent upward/downward movement from affecting the direction
        forward.Normalize();

        // Adjust the player's velocity in the direction the camera is facing
        Vector3 movement = forward * playerSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + movement);
    }
}