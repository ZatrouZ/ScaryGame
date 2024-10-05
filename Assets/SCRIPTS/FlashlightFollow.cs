using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    public Transform playerCamera;  // The camera that the flashlight will follow

    // Offset position relative to the camera, adjust this if necessary
    public Vector3 positionOffset = new Vector3(0.2f, -0.2f, 0.5f);

    // Optional offset for rotation if you want to adjust the flashlight's angle
    public Vector3 rotationOffset = new Vector3(0f, 0f, 0f);

    void Update()
    {
        // Update flashlight's position relative to the camera
        transform.position = playerCamera.position + playerCamera.TransformDirection(positionOffset);

        // Update flashlight's rotation to match the camera, with optional rotation offset
        transform.rotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);
    }
}
