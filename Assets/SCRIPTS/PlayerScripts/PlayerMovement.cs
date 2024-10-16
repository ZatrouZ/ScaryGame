using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    [Header("Footstep Sounds")]
    public AudioSource footstepAudioSource; // AudioSource for footsteps
    public AudioClip[] grassFootsteps; // Array of footstep sounds for grass
    public AudioClip[] woodFootsteps; // Array of footstep sounds for wood
    public AudioClip[] stoneFootsteps; // Array of footstep sounds for stone

    // Volume settings for each type of surface
    [Range(0, 1)] public float grassVolume = 1f;  // Volume for grass footsteps
    [Range(0, 1)] public float woodVolume = 1f;   // Volume for wood footsteps
    [Range(0, 1)] public float stoneVolume = 1f;  // Volume for stone footsteps

    public float walkFootstepSpeed = 1f;
    public float sprintFootstepSpeed = 1.5f;
    private bool isPlayingFootstepSound = false;

    private string currentSurfaceTag = "Default"; // Store the tag of the surface the player is walking on

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startYScale = transform.localScale.y;

        // Set up the footstep AudioSource
        footstepAudioSource.loop = true; // Loop the footsteps audio
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Detect surface and play footsteps
        DetectSurface();
        HandleFootsteps();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Detect surface type based on the object's tag
    private void DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 0.3f))
        {
            currentSurfaceTag = hit.collider.tag; // Get the tag of the object being hit by the raycast
        }
    }

    // Handle playing footstep sounds based on surface type
    private void HandleFootsteps()
    {
        if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            if (!isPlayingFootstepSound)
            {
                PlayFootstepSound(); // Play the appropriate footstep sound
                footstepAudioSource.Play();
                isPlayingFootstepSound = true;
            }

            if (state == MovementState.sprinting)
            {
                footstepAudioSource.pitch = sprintFootstepSpeed;
            }
            else
            {
                footstepAudioSource.pitch = walkFootstepSpeed;
            }
        }
        else
        {
            if (isPlayingFootstepSound)
            {
                footstepAudioSource.Stop();
                isPlayingFootstepSound = false;
            }
        }
    }

    // Play the correct footstep sound based on the current surface
    private void PlayFootstepSound()
    {
        AudioClip[] footstepClips = null;
        float volume = 1f;  // Default volume

        switch (currentSurfaceTag)
        {
            case "Grass":
                footstepClips = grassFootsteps;
                volume = grassVolume;
                break;
            case "Wood":
                footstepClips = woodFootsteps;
                volume = woodVolume;
                break;
            case "Stone": // Replaced "Metal" with "Stone"
                footstepClips = stoneFootsteps;
                volume = stoneVolume;
                break;
            default:
                footstepClips = grassFootsteps; // Default to grass if no tag matches
                volume = grassVolume;
                break;
        }

        if (footstepClips != null && footstepClips.Length > 0)
        {
            footstepAudioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.volume = volume;  // Apply the volume based on the surface type
        }
    }
}
