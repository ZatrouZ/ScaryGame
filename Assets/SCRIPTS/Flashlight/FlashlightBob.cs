using UnityEngine;

public class FlashlightBob : MonoBehaviour
{
    [Header("Flashlight Bob Settings")]
    public float walkBobSpeed = 4f;
    public float runBobSpeed = 7f;
    public float idleBobSpeed = 1f;
    public float walkBobAmount = 0.02f;
    public float runBobAmount = 0.05f;
    public float idleBobAmount = 0.01f;
    public float flashlightTiltAmount = 1.5f;

    private float timer = 0f;
    private Rigidbody playerRigidbody;

    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();  // Get the player's Rigidbody
    }

    public Vector3 GetBobbingOffset()
    {
        // Calculate player speed, ignoring vertical movement
        float speed = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z).magnitude;

        bool isMoving = speed > 0.1f;
        bool isRunning = speed > 5f;

        // Adjust bobbing speed based on movement state
        if (isMoving)
        {
            timer += Time.deltaTime * (isRunning ? runBobSpeed : walkBobSpeed);
        }
        else
        {
            timer += Time.deltaTime * idleBobSpeed;
        }

        // Calculate bob amount based on movement state
        float bobAmount = isRunning ? runBobAmount : (isMoving ? walkBobAmount : idleBobAmount);

        // Calculate the vertical and horizontal bobbing (y and x)
        float offsetY = Mathf.Sin(timer) * bobAmount;
        float offsetX = Mathf.Cos(timer * 2f) * (bobAmount / 2f);  // Smoother sideways bob

        // Return the calculated bobbing offset (only x and y, z remains unchanged)
        return new Vector3(offsetX, offsetY, 0);
    }

    public Quaternion GetBobbingRotation()
    {
        // Calculate the tilt effect for rotation (z-axis tilt)
        float tilt = Mathf.Sin(timer) * flashlightTiltAmount;

        // Return the calculated tilt as a quaternion (rotation)
        return Quaternion.Euler(0f, 0f, tilt);
    }
}

