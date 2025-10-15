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
    }

    private void Update()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = CoyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        _jumpBufferCounter -= Time.deltaTime;
        HandleJump();

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

            return; // skip normal movement this frame
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
        if (_rb.linearVelocity.y < 0)
            _rb.gravityScale = 3.5f;
        else
            _rb.gravityScale = 2.5f;
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
        {
            _jumpBufferCounter = JumpBufferTime;
        }
    }

    public void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (!CanMove) return;
        if (context.canceled && _rb.linearVelocity.y > 0f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * JumpCutMultiplier);
        }
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

    public void Launch(Vector2 velocity, float duration = 0.3f)
    {
        isLaunched = true;
        launchTimer = duration;
        _rb.linearVelocity = velocity;   // instantly apply launch
    }

}
