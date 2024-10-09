using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject flashlightObject; // Reference to the flashlight object (which contains the Light component)
    public KeyCode toggleKey = KeyCode.F;  // Key to toggle the flashlight

    private Light flashlightLight;  // Reference to the Light component inside the flashlight
    private bool isOn = true;  // Tracks whether the flashlight is on or off

    void Start()
    {
        if (flashlightObject != null)
        {
            // Get the Light component attached to the flashlight object
            flashlightLight = flashlightObject.GetComponent<Light>();
        }

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;  // Ensure the flashlight starts in the correct state
        }
        else
        {
            Debug.LogWarning("No Light component found on the flashlight object.");
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
        }
    }
}
