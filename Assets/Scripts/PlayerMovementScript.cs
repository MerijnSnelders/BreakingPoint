using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Player Settings")]
    [SerializeField] float Speed = 10;
    [SerializeField] float JumpPower = 15;

    [Header("Grounding")]
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] Transform GroundCheck;

    [Header("Jump Settings")]
    [SerializeField] float CoyoteTime = 0.15f;
    [SerializeField] float JumpBufferTime = 0.2f;
    [SerializeField] float JumpCutMultiplier = 0.5f;

    [Header("Footstep Audio")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField, Tooltip("Delay between footsteps when walking")]
    private float footstepInterval = 0.25f;
    private float footstepTimer;

    private bool isLaunched = false;
    private float launchTimer = 0f;
    private float _horizontalMovement;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    public bool CanMove { get; set; } = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 60;

        // Make sure the AudioSource is properly configured
        if (footstepSource != null)
        {
            footstepSource.playOnAwake = false;
            footstepSource.loop = false;
        }
    }

    private void Update()
    {
        if (IsGrounded())
            _coyoteTimeCounter = CoyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        _jumpBufferCounter -= Time.deltaTime;
        HandleJump();

        // Handle footsteps
        HandleFootsteps();

        // Update animation states
        animator.SetBool("isRunning", Mathf.Abs(_horizontalMovement) > 0.1f && IsGrounded());
        animator.SetBool("IsGrounded", IsGrounded());
        animator.SetFloat("YVelocity", _rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
        // Handle launch state (ignore input briefly)
        if (isLaunched)
        {
            launchTimer -= Time.fixedDeltaTime;
            if (launchTimer <= 0f)
                isLaunched = false;

            return;
        }

        if (!CanMove) return;

        // Normal movement
        _rb.linearVelocityX = _horizontalMovement;

        // Flip player
        if (_horizontalMovement > 0.1f)
            spriteRenderer.flipX = false;
        else if (_horizontalMovement < -0.1f)
            spriteRenderer.flipX = true;

        // Adjust gravity
        _rb.gravityScale = _rb.linearVelocity.y < 0 ? 3.5f : 2.5f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanMove) return;
        _horizontalMovement = context.ReadValue<Vector2>().x * Speed;
    }

    public void OnJumpPressed(InputAction.CallbackContext context)
    {
        if (!CanMove) return;
        if (context.performed)
            _jumpBufferCounter = JumpBufferTime;
    }

    public void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (!CanMove) return;
        if (context.canceled && _rb.linearVelocity.y > 0f)
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * JumpCutMultiplier);
    }

    private void HandleJump()
    {
        if (!CanMove) return;

        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, JumpPower);
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
    }

    private void HandleFootsteps()
    {
        bool isWalking = IsGrounded() && Mathf.Abs(_horizontalMovement) > 0.1f;

        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;

            // Time for the next step
            if (footstepTimer <= 0f)
            {
                footstepTimer = footstepInterval; // reset timer for next step

                if (footstepSource != null && footstepClip != null)
                {
                    StartCoroutine(PlayFootstepSegment());
                }
            }
        }
        else
        {
            // Stop footsteps immediately if not walking
            footstepTimer = 0f;
        }
    }

    private IEnumerator PlayFootstepSegment()
    {
        // Define the timestamps (in seconds) where each footstep starts.
        // Adjust these based on your clip’s waveform peaks!
        float[] stepTimes = { 0.09f, 0.7f, 1.3f, 1.8f, 2.4f, 3f };
        float stepLength = 0.15f; // how long one footstep lasts

        // Pick a random step from the list
        float startTime = stepTimes[Random.Range(0, stepTimes.Length)];

        footstepSource.clip = footstepClip;
        footstepSource.time = startTime;
        footstepSource.pitch = Random.Range(0.95f, 1.05f);
        footstepSource.Play();

        yield return new WaitForSeconds(stepLength);
        footstepSource.Stop();
    }





    public void Launch(Vector2 velocity, float duration = 0.3f)
    {
        isLaunched = true;
        launchTimer = duration;
        _rb.linearVelocity = velocity;
    }
}
