using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed; // The current movement speed, varies depending on the state (walking, sprinting, crouching)
    public float walkSpeed;  // Speed while walking
    public float sprintSpeed; // Speed while sprinting
    public float groundDrag; // Drag applied when on the ground to slow the player down

    [Header("Jumping")]
    public float jumpForce; // Force applied when jumping
    public float jumpCooldown; // Time between jumps to prevent continuous jumping
    public float airMultiplier; // Movement speed multiplier when in the air
    bool readyToJump; // Flag to determine if the player is ready to jump again

    [Header("Crouching")]
    public float crouchSpeed; // Speed while crouching
    public float crouchYScale; // The reduced height of the player when crouching
    private float startYScale; // The player's original height

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Key to trigger jumping
    public KeyCode sprintKey = KeyCode.LeftShift; // Key to trigger sprinting
    public KeyCode crouchKey = KeyCode.LeftControl; // Key to trigger crouching

    [Header("Ground Check")]
    public float playerHeight; // The height of the player, used for ground checks
    public LayerMask whatIsGround; // What is considered ground for the player
    bool grounded; // Is the player currently on the ground?

    [Header("Slope Handling")]
    public float maxSlopeAngle; // The maximum angle the player can walk on without sliding
    private RaycastHit slopeHit; // Information about the slope the player is currently on
    private bool exitingSlope; // Flag to prevent slope issues when jumping

    public Transform orientation; // Orientation used to determine movement direction

    float horizontalInput; // Horizontal movement input (A/D or Left/Right keys)
    float verticalInput; // Vertical movement input (W/S or Up/Down keys)

    Vector3 moveDirection; // The direction the player is moving in

    Rigidbody rb; // Rigidbody component for controlling player physics

    public MovementState state; // The player's current movement state
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents the player from rotating due to physics

        readyToJump = true; // Player starts ready to jump

        startYScale = transform.localScale.y; // Save the original height of the player
    }

    private void Update()
    {
        // Check if the player is grounded using a raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Handle inputs and state updates
        MyInput();
        SpeedControl();
        StateHandler();

        // Apply drag when grounded, remove drag when in the air
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        // Call the movement function every physics update
        MovePlayer();
    }

    private void MyInput()
    {
        // Capture horizontal and vertical input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump when the jump key is pressed and the player is grounded and ready to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false; // Disable jumping until cooldown is over

            Jump(); // Perform the jump

            Invoke(nameof(ResetJump), jumpCooldown); // Reset the jump after the cooldown
        }

        // Start crouching when crouch key is pressed
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z); // Lower the player's height
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Force downwards to stick the player to the ground
        }

        // Stop crouching when crouch key is released
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z); // Restore the original height
        }
    }

    private void StateHandler()
    {
        // Crouching state
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Sprinting state
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Walking state
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Airborne state (jumping or falling)
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Calculate the movement direction based on player input and orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Movement on a slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force); // Apply force in slope direction

            // Counteract vertical movement on the slope
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // Movement on flat ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // Movement in the air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Disable gravity on slopes
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // Limit speed on slopes
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        // Limit speed on flat ground or in the air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Get only horizontal velocity

            // If speed exceeds max, limit it
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // Apply limited velocity
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true; // Indicate that we are leaving a slope

        // Reset vertical velocity before jumping
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add upward force for jumping
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true; // Reset jump flag after cooldown
        exitingSlope = false; // No longer exiting a slope
    }

    // Check if the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0; // True if slope angle is within acceptable range
        }

        return false; // Not on a slope
    }

    // Get movement direction on a slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized; // Adjust movement to follow the slope
    }
}
