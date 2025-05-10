using UnityEngine;

public class DoorActivator : MonoBehaviour
{
    public int totalSlimes = 4; // Ajusta según la cantidad de slimes
    private int slimesDead = 0;
    private Collider doorCollider;

    void Start()
    {
        doorCollider = GetComponent<Collider>();
        doorCollider.enabled = false; // Desactiva la puerta al inicio
    }

    // Llama esto desde EnemyHealth cuando un slime muere
    public void SlimeDied()
    {
        slimesDead++;
        if (slimesDead >= totalSlimes)
        {
            doorCollider.enabled = true; // Activa la puerta
            // Aquí puedes activar efectos visuales, sonido, etc.
        }
    }
}