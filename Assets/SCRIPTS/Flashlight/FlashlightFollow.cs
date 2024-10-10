using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    public Transform playerCamera;  // The camera that the flashlight will follow
    public Vector3 positionOffset = new Vector3(0.4f, -0.25f, 0.5f);  // Position offset relative to the camera
    public Vector3 rotationOffset = new Vector3(80f, -2f, -4f);  // Rotation offset for flashlight angle

    public float positionSmoothSpeed = 15f;  // Speed for smoothing the position
    public float rotationSmoothSpeed = 15f;  // Speed for smoothing the rotation

    void Update()
    {
        // Calculate target position relative to the camera
        Vector3 targetPosition = playerCamera.position + playerCamera.TransformDirection(positionOffset);

        // Smoothly move the flashlight to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

        // Calculate target rotation based on the camera's rotation and rotationOffset
        Quaternion targetRotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);

        // Smoothly rotate the flashlight to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
