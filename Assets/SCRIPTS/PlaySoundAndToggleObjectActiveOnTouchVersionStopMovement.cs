using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAndToggleObjectActiveOnTouchVersionStopMovement : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip soundClip;              // Assign the sound clip in the Inspector
    private AudioSource audioSource;         // Reference to the AudioSource component

    [Header("Objects To Toggle")]
    public List<GameObject> objectsToToggle; // List of GameObjects to toggle active/inactive

    [Header("Camera Focus Settings")]
    public Transform playerCamera;           // The player's camera
    public Transform playerBody;             // Reference to the player's body (for movement direction)
    public float focusSpeed = 2f;            // Speed at which the camera focuses on the object
    public float focusDuration = 5f;         // Duration to hold the camera focus
    public MonoBehaviour cameraBobScript;    // Reference to the Camera Bob script to deactivate

    [Header("Player Movement Settings")]
    public MonoBehaviour playerMovementScript; // Reference to the player's movement script (e.g., FPS controller or movement system)

    private Quaternion originalCameraRotation; // To store the original camera rotation
    private Transform targetObject;          // The object to focus on

    void Start()
    {
        // Ensure there's an AudioSource component on the GameObject (set in the Inspector)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = soundClip; // Assign the sound clip to the AudioSource
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player")) // Ensure your player GameObject has the "Player" tag
        {
            // Play sound and toggle objects
            PlaySound();
            ToggleObjects();

            // Find the first toggled object and focus the camera
            if (objectsToToggle.Count > 0)
            {
                targetObject = objectsToToggle[0].transform; // Focus on the first object
                originalCameraRotation = playerCamera.rotation; // Store the original camera rotation
                StartCoroutine(FocusOnObject());
            }
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

    void ToggleObjects()
    {
        // Toggle the active state of the objects in the list
        foreach (GameObject obj in objectsToToggle)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    IEnumerator FocusOnObject()
    {
        // Disable the camera bob script and player movement while focusing
        if (cameraBobScript != null)
        {
            cameraBobScript.enabled = false;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;  // Disable player movement
        }

        // Calculate the direction to the target object (toggled object)
        Vector3 directionToTarget = (targetObject.position - playerCamera.position).normalized;

        // Start focusing on the object
        float elapsedTime = 0f;

        while (elapsedTime < focusDuration)
        {
            elapsedTime += Time.deltaTime;

            // Smoothly rotate the camera to look at the target object
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, Time.deltaTime * focusSpeed);

            // Check if the camera is very close to looking directly at the object
            float angle = Quaternion.Angle(playerCamera.rotation, targetRotation);
            if (angle < 1f)
            {
                break; // Stop focusing if close enough to the target rotation
            }

            yield return null;
        }

        // Force the player's body (movement) to align with the camera's final direction
        playerBody.rotation = playerCamera.rotation;  // Align the player's body with the camera's rotation

        // Wait for the focus duration before re-enabling movement
        yield return new WaitForSeconds(focusDuration);

        // Smoothly transition the camera back to its original rotation
        float returnDuration = 1.5f; // Duration for the return transition
        elapsedTime = 0f;

        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate back to the original camera rotation
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, originalCameraRotation, (elapsedTime / returnDuration));

            yield return null;
        }

        // Ensure the camera is exactly at the original rotation after the return
        playerCamera.rotation = originalCameraRotation;

        // Re-enable the camera bob script and player movement after focusing
        if (cameraBobScript != null)
        {
            cameraBobScript.enabled = true;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;  // Re-enable player movement immediately
        }

        // Destroy the toggled object(s)
        foreach (GameObject obj in objectsToToggle)
        {
            Destroy(obj); // Destroy the toggled object
        }

        // Destroy this GameObject (the one holding the script)
        Destroy(gameObject);
    }
}