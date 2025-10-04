using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    private Rigidbody2D _rb;

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

    private float _horizontalMovement;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    public bool CanMove { get; set; } = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;
        _rb.linearVelocityX = _horizontalMovement;

        if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = 3.5f;
        }
        else
        {
            _rb.gravityScale = 2.5f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanMove) return;
        _horizontalMovement = context.ReadValue<Vector2>().x * Speed;
    }

    public void OnJumpPressed(InputAction.CallbackContext context)
    {
        Debug.Log("jump");
        if (!CanMove) return;
        if (context.performed)
        {
            Debug.Log("jump2");
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

}
