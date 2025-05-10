using UnityEngine;

public class CameraFollowSmoothXZ : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Arrastra el Transform del Jugador aquí

    [Header("Camera Settings")]
    public float smoothTime = 0.3f;
    public Vector3 offset;

    [Header("Camera Limits (Clamping)")]
    public bool enableLimits = true; // Para activar/desactivar fácilmente los límites
    public float minX = -10f;       // Límite mínimo en X
    public float maxX = 10f;        // Límite máximo en X
    public float minZ = -15f;       // Límite mínimo en Z
    public float maxZ = 5f;         // Límite máximo en Z

    // Variables privadas
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Asigna un Target (Jugador)...", this);
            enabled = false;
            return;
        }
        // Calcula offset inicial (¡Posiciona bien la cámara ANTES de Play!)
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- Calcular Posición Deseada ---
        Vector3 targetPosition = target.position + offset;

        // --- Aplicar Movimiento Suave ---
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // --- Aplicar Límites (Clamping) ---
        if (enableLimits)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.z = Mathf.Clamp(smoothedPosition.z, minZ, maxZ);
            // No limitamos Y porque queremos mantener la altura definida por el offset inicial
        }

        // --- Aplicar Posición Final ---
        transform.position = smoothedPosition;

        // Opcional: Mantener rotación fija
        // transform.rotation = Quaternion.Euler(25f, 0f, 0f); // Ejemplo
    }

    // (Opcional) Dibuja los límites en el editor para visualizarlos
    void OnDrawGizmosSelected()
    {
        if (enableLimits)
        {
            // Calcula el tamaño del rectángulo de límites
            float width = maxX - minX;
            float depth = maxZ - minZ;
            // Calcula el centro del rectángulo
            Vector3 center = new Vector3(minX + width / 2, transform.position.y - offset.y, minZ + depth / 2); // Usamos la Y del jugador como referencia

            // Dibuja un cubo alámbrico representando los límites XZ a la altura aproximada del jugador
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, new Vector3(width, 1f, depth)); // El '1f' en Y es solo para darle algo de altura visible al gizmo
        }
    }
}