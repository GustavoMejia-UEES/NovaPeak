using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Asegura que siempre haya un Rigidbody
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    [Tooltip("Invierte los controles Izquierda/Derecha")]
    public bool invertXAxis = false;
    [Tooltip("Invierte los controles Adelante/Atrás")]
    public bool invertZAxis = false;

    [Header("Ground Check")]
    public Transform groundCheckPoint; // Renombrado para claridad
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Componentes y estado
    private Rigidbody rb;
    private PlayerInputHandler inputHandler; // Referencia al manejador de inputs
    private bool isFacingRight = true;
    public bool IsGrounded { get; private set; }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<PlayerInputHandler>(); // Obtiene el manejador de inputs

        if (inputHandler == null)
        {
            Debug.LogError("PlayerMotor necesita un PlayerInputHandler en el mismo GameObject.", this);
            enabled = false; // Desactiva si no hay input handler
            return;
        }
        if (groundCheckPoint == null)
        {
            Debug.LogError("Asigna un Transform al 'Ground Check Point' en PlayerMotor.", this);
            enabled = false;
            return;
        }

        rb.freezeRotation = true; // Congela rotación por física
    }

    void Update()
    {
        // Realiza el chequeo de suelo
        CheckIfGrounded();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    void CheckIfGrounded()
    {
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    void HandleMovement()
    {
        float processedX = inputHandler.MoveInput.x;
        float processedZ = inputHandler.MoveInput.y;

        if (invertXAxis) processedX *= -1f;
        if (invertZAxis) processedZ *= -1f;

        Vector3 targetVelocity = new Vector3(processedX * moveSpeed, rb.linearVelocity.y, processedZ * moveSpeed);
        rb.linearVelocity = targetVelocity;

        // Manejar el volteo del personaje
        if (Mathf.Abs(processedX) > 0.01f) // Solo voltear si hay input horizontal significativo
        {
            bool shouldFaceRight = processedX > 0;
            if (shouldFaceRight != isFacingRight)
            {
                Flip();
            }
        }
    }

    void HandleJump()
    {
        if (inputHandler.JumpTriggered && IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            inputHandler.ResetJumpTrigger(); // Importante resetear el trigger después de usarlo
        }
        // Si el salto no se usó, pero el trigger estaba activo, igual resetéalo para evitar saltos en el siguiente frame
        else if (inputHandler.JumpTriggered)
        {
            inputHandler.ResetJumpTrigger();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Gizmos para visualizar el GroundCheck
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}