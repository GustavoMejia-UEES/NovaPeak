using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // <-- Importante para recargar la escena

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Coroutine flashCoroutine;
    private Color originalColor;
    private HealthBar healthBar;

    [Header("Health Regeneration")]
    public bool enableRegeneration = true;
    public int regenAmount = 1;
    public float regenInterval = 2f;
    private float lastRegenTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        // Buscar el HealthBar en los hijos
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
    }

    void Update()
    {
        if (enableRegeneration && currentHealth < maxHealth && Time.time >= lastRegenTime + regenInterval)
        {
            currentHealth += regenAmount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            if (healthBar != null) healthBar.SetHealth(currentHealth, maxHealth);
            lastRegenTime = Time.time;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Player took " + amount + " damage. Current Health: " + currentHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        FlashRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void FlashRed(float duration = 0.15f)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashCoroutine(sr, duration));
        }
    }

    private IEnumerator FlashCoroutine(SpriteRenderer sr, float duration)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(duration);
        sr.color = originalColor;
    }
}