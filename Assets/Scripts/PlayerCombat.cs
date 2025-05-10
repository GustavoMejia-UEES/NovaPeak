using UnityEngine;
using System.Collections; // Para Coroutines (usado en Parry)

[RequireComponent(typeof(PlayerInputHandler))] // Necesita leer los inputs de acción
[RequireComponent(typeof(PlayerAnimator))]    // Podría necesitar interactuar o esperar animaciones (opcional)
public class PlayerCombat : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform muzzlePoint;
    public float shootCooldown = 0.5f; // Tiempo entre disparos
    private float lastShootTime = -Mathf.Infinity;

    [Header("Melee")]
    public float meleeAttackRange = 1.5f;
    public float meleeAttackOffset = 0.7f;
    public int meleeDamage = 25;
    public LayerMask enemyLayer; // Capa para identificar enemigos
    public float meleeCooldown = 0.8f;
    private float lastMeleeTime = -Mathf.Infinity;


    [Header("Parry")]
    public float parryDuration = 0.3f;
    public float parryCooldown = 1.0f;
    private float lastParryTime = -Mathf.Infinity;
    private bool isCurrentlyParrying = false; // Estado interno del parry

    // Referencias a otros componentes
    private PlayerInputHandler inputHandler;
    private Animator animator;
    private readonly int shootTriggerHash = Animator.StringToHash("Shoot");
    private readonly int meleeTriggerHash = Animator.StringToHash("Melee");
    private readonly int parryTriggerHash = Animator.StringToHash("Parry");
    // private PlayerAnimator playerAnimator; // Descomentar si necesitas referenciarlo

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        // playerAnimator = GetComponent<PlayerAnimator>(); // Descomentar si es necesario

        if (inputHandler == null)
            Debug.LogError("PlayerCombat necesita un PlayerInputHandler.", this);
        // if (playerAnimator == null)
        // Debug.LogWarning("PlayerCombat no encontró PlayerAnimator (opcional).", this);

        // Validar referencias de combate
        if (projectilePrefab == null) Debug.LogWarning("Projectile Prefab no asignado en PlayerCombat.", this);
        if (muzzlePoint == null) Debug.LogWarning("Muzzle Point no asignado en PlayerCombat.", this);
    }

    void Update()
    {
        // Escuchar los triggers de input y solo activar el trigger del Animator
        HandleCombatInputs();
    }

    void HandleCombatInputs()
    {
        if (inputHandler.ShootTriggered && Time.time >= lastShootTime + shootCooldown)
        {
            if (animator != null) animator.SetTrigger(shootTriggerHash);
            lastShootTime = Time.time;
            inputHandler.ResetShootTrigger();
        }

        if (inputHandler.MeleeTriggered && Time.time >= lastMeleeTime + meleeCooldown)
        {
            if (animator != null) animator.SetTrigger(meleeTriggerHash);
            lastMeleeTime = Time.time;
            inputHandler.ResetMeleeTrigger();
        }

        if (inputHandler.ParryTriggered && !isCurrentlyParrying && Time.time >= lastParryTime + parryCooldown)
        {
            if (animator != null) animator.SetTrigger(parryTriggerHash);
            StartCoroutine(PerformParryCoroutine());
            lastParryTime = Time.time;
            // No reseteamos el inputHandler.ParryTriggered aquí, se hará en la corrutina
        }
        else if (inputHandler.ParryTriggered)
        {
            inputHandler.ResetParryTrigger();
        }
    }

    IEnumerator PerformParryCoroutine()
    {
        // El trigger del Animator lo maneja PlayerAnimator.cs
        Debug.Log("PlayerCombat: Parry Start!");
        isCurrentlyParrying = true;

        // Reseteamos el input aquí porque la acción de parry ha comenzado
        if (inputHandler.ParryTriggered) inputHandler.ResetParryTrigger();

        // Aquí puedes activar visuales/efectos de parry si los tienes

        yield return new WaitForSeconds(parryDuration);

        isCurrentlyParrying = false;
        Debug.Log("PlayerCombat: Parry End!");
    }

    // Función pública para que otros scripts (ej: proyectiles enemigos) comprueben
    public bool IsCurrentlyParrying()
    {
        return isCurrentlyParrying;
    }


    // Gizmos para visualizar rangos de combate
    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        // Melee Gizmo
        if (Application.isPlaying || transform.hasChanged) // Solo si está en play o ha cambiado para que transform.forward sea útil
        {
            Gizmos.color = Color.red;
            Vector3 attackCenter = transform.position + transform.forward * meleeAttackOffset;
            Gizmos.DrawWireSphere(attackCenter, meleeAttackRange);
        }

        // MuzzlePoint Gizmo (si es visible y quieres dibujarlo)
        // if (muzzlePoint != null)
        // {
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawSphere(muzzlePoint.position, 0.1f);
        // }
#endif
    }

    // PerformShoot and PerformMelee are now called by animation events
    public void OnShootAnimationEvent()
    {
        Debug.Log("PlayerCombat: Performing Shoot!");
        if (projectilePrefab != null && muzzlePoint != null)
        {
            Vector3 shootDirection = transform.right;
            GameObject projectileObj = Instantiate(projectilePrefab, muzzlePoint.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetDirection(shootDirection);
            }
            else
            {
                Debug.LogError("Projectile prefab is missing the Projectile component!", this);
            }
        }
    }

    public void OnMeleeAnimationEvent()
    {
        Debug.Log("PlayerCombat: Performing Melee!");
        Vector3 attackCenter = transform.position + transform.forward * meleeAttackOffset;
        Collider[] hitColliders;
        if (enemyLayer.value != 0)
            hitColliders = Physics.OverlapSphere(attackCenter, meleeAttackRange, enemyLayer);
        else
            hitColliders = Physics.OverlapSphere(attackCenter, meleeAttackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == this.gameObject) continue;
            Debug.Log("Melee Hit: " + hitCollider.name);
            EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }
    }
}