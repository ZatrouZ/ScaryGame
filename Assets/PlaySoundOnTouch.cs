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
            PlaySound(); // Play the sound effect
            ToggleObjects(); // Toggle the chosen GameObjects
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
}
