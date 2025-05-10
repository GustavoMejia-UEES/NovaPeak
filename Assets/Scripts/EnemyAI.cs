using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f; // Distancia mínima para detenerse (rango de ataque)
    public float detectionDistance = 6f; // Distancia para despertar
    public int damage = 10;
    public float attackCooldown = 1.0f;
    public float attackRadius = 1.5f; // Radio de daño en área
    public LayerMask playerLayer; // Asigna la capa del jugador en el Inspector
    private float lastAttackTime = -Mathf.Infinity;

    private Transform player;
    private PlayerHealth playerHealth;
    private SlimeAnimator slimeAnimator;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isHurting = false;
    private bool isAwake = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
        slimeAnimator = GetComponent<SlimeAnimator>();
    }

    void Update()
    {
        if (player == null || isDead || isHurting) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Si no está despierto, solo revisa si el jugador está cerca
        if (!isAwake)
        {
            if (distance <= detectionDistance)
            {
                isAwake = true;
            }
            else
            {
                if (slimeAnimator != null) slimeAnimator.PlayIdle();
                return; // No hace nada más
            }
        }

        // Si está despierto, sigue la lógica normal
        if (distance > stopDistance)
        {
            // Movimiento
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (slimeAnimator != null && !isAttacking) slimeAnimator.PlayIdle();
        }
        else
        {
            // Ataca si está en rango, no está atacando, no está herido y el cooldown lo permite
            if (!isAttacking && !isHurting && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        if (slimeAnimator != null) slimeAnimator.PlayAttack();
        yield return new WaitForSeconds(0.5f);

        // Daño en área usando OverlapSphere
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        foreach (var hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    // Llamar esto desde EnemyHealth cuando recibe daño
    public void OnHurt(float duration)
    {
        if (isDead) return;
        StartCoroutine(HurtCoroutine(duration));
    }

    IEnumerator HurtCoroutine(float duration)
    {
        isHurting = true;
        if (slimeAnimator != null) slimeAnimator.PlayHurt(duration);
        yield return new WaitForSeconds(duration);
        isHurting = false;
    }

    // Llamar esto desde EnemyHealth cuando muere
    public void OnDeath()
    {
        isDead = true;
        StopAllCoroutines();
        if (slimeAnimator != null) slimeAnimator.PlayDeath();
    }
}