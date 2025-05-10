using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))] // Depende de PlayerMotor para IsGrounded
[RequireComponent(typeof(PlayerInputHandler))] // Depende de PlayerInputHandler para los triggers de acción
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerMotor motor;
    private PlayerInputHandler inputHandler;
    private Rigidbody rb; // Para calcular la velocidad horizontal

    // Hashes de los parámetros del Animator para eficiencia (opcional pero buena práctica)
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded"); // Asume que tienes este parámetro Bool
    private readonly int shootTriggerHash = Animator.StringToHash("Shoot");
    private readonly int meleeTriggerHash = Animator.StringToHash("Melee");
    private readonly int parryTriggerHash = Animator.StringToHash("Parry");
    // Añade aquí hashes para otros triggers o parámetros si los tienes (ej: JumpTrigger)

    void Awake()
    {
        animator = GetComponent<Animator>();
        motor = GetComponent<PlayerMotor>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>(); // El Rigidbody está en el mismo objeto que el motor

        if (animator == null) Debug.LogError("PlayerAnimator necesita un Animator.", this);
        if (motor == null) Debug.LogError("PlayerAnimator necesita un PlayerMotor.", this);
        if (inputHandler == null) Debug.LogError("PlayerAnimator necesita un PlayerInputHandler.", this);
        if (rb == null) Debug.LogError("PlayerAnimator necesita un Rigidbody para calcular la velocidad.", this);
    }

    void Update()
    {
        // Actualizar parámetros básicos del Animator
        if (animator == null) return;

        // Velocidad horizontal
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        animator.SetFloat(speedHash, horizontalSpeed);

        // Estado de IsGrounded (obtenido del PlayerMotor)
        animator.SetBool(isGroundedHash, motor.IsGrounded);
    }
}