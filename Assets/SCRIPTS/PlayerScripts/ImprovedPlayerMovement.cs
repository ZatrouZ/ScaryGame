using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;  // Speed while walking
    public float sprintSpeed = 8f; // Speed while sprinting
    public float crouchSpeed = 2.5f; // Speed while crouching
    private float moveSpeed;  // The current movement speed
    private Vector3 moveDirection;  // The direction the player is moving

    [Header("Jumping")]
    public float jumpForce = 5f;  // Force applied when jumping
    public float jumpCooldown = 0.2f; // Cooldown between jumps
    public float gravity = -9.81f;  // Gravity value
    public bool readyToJump = true; // Is the player ready to jump

    [Header("Crouching")]
    public float crouchYScale = 0.5f; // Crouch scale for the player's height
    private float originalYScale;  // Original scale before crouching

    [Header("Ground Check")]
    public Transform groundCheck; // Ground check position
    public float groundDistance = 0.4f; // Distance for ground detection
    public LayerMask groundMask;  // LayerMask to define what is considered ground
    private bool isGrounded; // Is the player grounded?

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;  // Key to trigger jumping
    public KeyCode sprintKey = KeyCode.LeftShift;  // Key to trigger sprinting
    public KeyCode crouchKey = KeyCode.LeftControl;  // Key to trigger crouching

    private CharacterController controller;  // The CharacterController component
    private Vector3 velocity;  // Velocity used for vertical movement (jumping, gravity)
    public Transform orientation;  // Orientation to determine forward direction

    void Start()
    {
        // Get the CharacterController component
        controller = GetComponent<CharacterController>();

        // Store the original scale of the player (for crouching)
        originalYScale = transform.localScale.y;
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset vertical velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Small negative value to ensure the player stays grounded
        }

        // Handle movement input and apply gravity, jump logic
        MyInput();
        MovePlayer();
        HandleCrouch();
        ApplyGravity();
    }

    void MyInput()
    {
        // Capture player input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();

        // Sprint
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            moveSpeed = crouchSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        // Jump logic
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void MovePlayer()
    {
        // Move the player in the calculated direction (only X and Z, Y is handled separately for gravity and jumping)
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        // Apply gravity to the velocity
        velocity.y += gravity * Time.deltaTime;

        // Apply the velocity to the CharacterController
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        // Add jump force to the vertical velocity
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            // Reduce player height when crouching
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            // Reset player height when stopping crouch
            transform.localScale = new Vector3(transform.localScale.x, originalYScale, transform.localScale.z);
        }
    }
}
