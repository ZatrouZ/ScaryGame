using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTouch : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip soundClip;  // Assign the sound clip in the Inspector
    private AudioSource audioSource; // Reference to the AudioSource component

    [Header("Objects to Toggle")]
    public List<GameObject> objectsToToggle; // List of GameObjects to be toggled on touch

    [Header("Camera Focus Settings")]
    public Camera playerCamera;             // Reference to the player camera
    public float cameraFocusSpeed = 2f;     // Speed of camera movement when focusing on the object
    private bool focusOnObject = false;     // Flag to trigger camera focus
    private Transform targetObject;         // The object that the camera should look at
    public float focusDuration = 5f;        // Duration for which the camera will focus on the object (5 seconds)

    void Start()
    {
        // Ensure there's an AudioSource component on the GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip; // Assign the sound clip to the AudioSource
    }

    private void Update()
    {
        // If camera needs to focus on an object, smoothly rotate to look at it
        if (focusOnObject && targetObject != null)
        {
            Vector3 directionToTarget = (targetObject.position - playerCamera.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, lookRotation, Time.deltaTime * cameraFocusSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player")) // Ensure your player GameObject has the "Player" tag
        {
            PlaySound(); // Play the sound effect
            ToggleObjects(); // Toggle the chosen GameObjects

            // Focus the camera on the first toggled object
            if (objectsToToggle.Count > 0 && objectsToToggle[0].activeSelf)
            {
                StartCameraFocus(objectsToToggle[0].transform); // Start focusing on the first object
                StartCoroutine(StopCameraFocusAfterDelay(focusDuration)); // Stop focusing after 5 seconds
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
        // Loop through the list of GameObjects and toggle their active state
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null) // Ensure the object is assigned
            {
                bool isActive = obj.activeSelf; // Get the current active state
                obj.SetActive(!isActive); // Toggle the active state
            }
            else
            {
                Debug.LogWarning("One of the objects to toggle is not assigned in " + gameObject.name);
            }
        }
    }

    // Start focusing the camera on a specific target
    void StartCameraFocus(Transform target)
    {
        targetObject = target; // Set the target object to focus on
        focusOnObject = true;  // Enable camera focus
    }

    // Stop camera focus after a delay (5 seconds)
    IEnumerator StopCameraFocusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        StopCameraFocus(); // Stop focusing the camera
    }

    // Stop focusing the camera on the object
    void StopCameraFocus()
    {
        focusOnObject = false;  // Disable camera focus
        targetObject = null;    // Clear the target
    }
}
