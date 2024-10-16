using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject flashlightObject;  // Reference to the flashlight object (which contains the Light component)
    public KeyCode toggleKey = KeyCode.F;  // Key to toggle the flashlight
    public AudioClip toggleOnSound;  // Sound to play when the flashlight turns on
    public AudioClip toggleOffSound; // Sound to play when the flashlight turns off

    private Light flashlightLight;  // Reference to the Light component inside the flashlight
    private AudioSource audioSource;  // Reference to the AudioSource for playing sounds
    private bool isOn = true;  // Tracks whether the flashlight is on or off

    void Start()
    {
        if (flashlightObject != null)
        {
            // Get the Light component attached to the flashlight object
            flashlightLight = flashlightObject.GetComponent<Light>();
        }

        // Ensure the Light component exists
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;  // Ensure the flashlight starts in the correct state
        }
        else
        {
            Debug.LogWarning("No Light component found on the flashlight object.");
        }

        // Get or add an AudioSource component for playing the toggle sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Check if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        // Toggle the flashlight state
        isOn = !isOn;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;  // Enable or disable the light component

            // Play the appropriate toggle sound based on whether the flashlight is on or off
            PlayToggleSound();
        }
    }

    void PlayToggleSound()
    {
        // If flashlight is turned on, play the "on" sound, otherwise play the "off" sound
        if (isOn && toggleOnSound != null)
        {
            audioSource.PlayOneShot(toggleOnSound);
        }
        else if (!isOn && toggleOffSound != null)
        {
            audioSource.PlayOneShot(toggleOffSound);
        }
    }
}
