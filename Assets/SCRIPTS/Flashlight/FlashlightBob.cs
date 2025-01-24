using UnityEngine;

public class FlashlightBob : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera's transform to sync with its bobbing
    public float flashlightTiltAmount = 1.5f;  // Tilt amount for flashlight bobbing
    public float bobSmoothing = 10f;  // Smoothing factor for bobbing

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 currentBobOffset;  // Smoothed bobbing offset
    private Quaternion currentBobRotation;  // Smoothed bobbing rotation

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        ApplyBobbing();
    }

    void ApplyBobbing()
    {
        // Sync flashlight with the camera's bobbing movement
        //Vector3 targetPosition = cameraTransform.localPosition + initialLocalPosition;

        // Add small tilt to make flashlight bob unique from the camera
        float tilt = Mathf.Sin(Time.time * 10f) * flashlightTiltAmount;  // Arbitrary factor for tilting
        Quaternion targetRotation = cameraTransform.localRotation * Quaternion.Euler(0, 0, tilt);

        // Smoothly apply the bobbing and tilting to the flashlight
        //currentBobOffset = Vector3.Lerp(currentBobOffset, targetPosition, Time.deltaTime * bobSmoothing);
        currentBobRotation = Quaternion.Slerp(currentBobRotation, targetRotation, Time.deltaTime * bobSmoothing);

        // Apply the smoothed position and rotation to the flashlight
        transform.localPosition = currentBobOffset;
        transform.localRotation = currentBobRotation;
    }
}

