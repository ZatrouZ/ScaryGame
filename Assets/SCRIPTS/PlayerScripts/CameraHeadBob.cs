using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    public float walkBobSpeed = 4f;
    public float runBobSpeed = 7f;
    public float idleBobSpeed = 1f;
    public float walkBobAmount = 0.02f;
    public float runBobAmount = 0.05f;
    public float idleBobAmount = 0.01f;

    private float timer = 0f;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        HeadBob();
    }

    void HeadBob()
    {
        // Calculate the player's movement speed (this can vary based on your player script)
        float speed = Mathf.Abs(GetComponentInParent<Rigidbody>().velocity.magnitude);
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

        // Calculate bobbing amount based on movement state
        float bobAmount = isRunning ? runBobAmount : (isMoving ? walkBobAmount : idleBobAmount);

        // Apply bobbing to camera's position
        Vector3 newPosition = initialPosition + new Vector3(0, Mathf.Sin(timer) * bobAmount, 0);
        transform.localPosition = newPosition;
    }
}
