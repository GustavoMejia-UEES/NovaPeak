using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 3f;
    public LayerMask targetLayers;
    public LayerMask obstacleLayers;
    public GameObject hitEffectPrefab;

    private Vector3 direction;
    private bool hasHit = false;

    void Start()
    {
        // Destroy the projectile after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        // Rotate the projectile to face the direction it's moving
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (!hasHit)
        {
            // Move the projectile
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Check if we hit an obstacle
        if (((1 << other.gameObject.layer) & obstacleLayers) != 0)
        {
            HandleHit(other.gameObject);
            return;
        }

        // Check if we hit a valid target
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // Try to damage the target
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Mathf.RoundToInt(damage));
            }
            HandleHit(other.gameObject);
        }
    }

    void HandleHit(GameObject hitObject)
    {
        hasHit = true;

        // Spawn hit effect if we have one
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the projectile
        Destroy(gameObject);
    }

    // Optional: Visualize the projectile's path in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * speed);
    }
} 