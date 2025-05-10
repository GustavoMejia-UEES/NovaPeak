using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float hurtDuration = 0.4f;
    public float deathDuration = 0.7f;
    private SlimeAnimator slimeAnimator;
    private EnemyAI enemyAI;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        slimeAnimator = GetComponent<SlimeAnimator>();
        enemyAI = GetComponent<EnemyAI>();

        // Buscar el HealthBar en los hijos
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);
        
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (enemyAI != null) enemyAI.OnHurt(hurtDuration);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        if (enemyAI != null) enemyAI.OnDeath();
        if (slimeAnimator != null) slimeAnimator.PlayDeath(deathDuration);
        FindAnyObjectByType<DoorActivator>()?.SlimeDied();
    }
}