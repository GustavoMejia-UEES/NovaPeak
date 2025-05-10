using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer fillSprite;
    private Vector3 originalScale;
    private bool isVisible = true;

    void Start()
    {
        // Buscar automáticamente el HealthBarFill
        fillSprite = transform.Find("HealthBarFill")?.GetComponent<SpriteRenderer>();
        if (fillSprite != null)
        {
            originalScale = fillSprite.transform.localScale;
        }
        else
        {
            Debug.LogError("No se encontró HealthBarFill en " + gameObject.name);
        }
    }

    public void SetHealth(int current, int max)
    {
        if (fillSprite == null) return;

        float healthPercent = (float)current / max;
        
        // Ocultar/mostrar la barra según la vida
        bool shouldBeVisible = current < max;
        if (shouldBeVisible != isVisible)
        {
            isVisible = shouldBeVisible;
            fillSprite.gameObject.SetActive(isVisible);
            transform.Find("HealthBarBackground")?.gameObject.SetActive(isVisible);
        }

        if (isVisible)
        {
            // Ajustar la escala X manteniendo Y y Z originales
            fillSprite.transform.localScale = new Vector3(
                originalScale.x * healthPercent,
                originalScale.y,
                originalScale.z
            );
        }
    }

    void LateUpdate()
    {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }
}