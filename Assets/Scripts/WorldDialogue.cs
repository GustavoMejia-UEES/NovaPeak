using UnityEngine;
using TMPro;

public class WorldDialogue : MonoBehaviour
{
    public GameObject dialogueContent; // Padre de sprites/textos
    public Transform player; // Asigna el jugador en el Inspector
    public bool onlyY = true; // Solo rota en Y
    public float autoHideTime = 0f;

    [TextArea(2, 5)]
    public string message; // Solo si usas texto
    public TextMeshPro textMesh; // Solo si usas texto

    void Start()
    {
        if (textMesh != null)
            SetText(message);
        Hide();
    }

    void Update()
    {
        // Rotar el cartel para mirar al jugador
        if (player != null && dialogueContent != null && dialogueContent.activeSelf)
        {
            Vector3 lookPos = player.position - dialogueContent.transform.position;
            if (onlyY) lookPos.y = 0;
            if (lookPos.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(lookPos);
                dialogueContent.transform.rotation = rot;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Show();
            if (textMesh != null) SetText(message);
            if (autoHideTime > 0)
            {
                CancelInvoke(nameof(Hide));
                Invoke(nameof(Hide), autoHideTime);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Hide();
        }
    }

    public void SetText(string newText)
    {
        message = newText;
        if (textMesh != null)
            textMesh.text = message;
    }

    public void Show()
    {
        if (dialogueContent != null)
            dialogueContent.SetActive(true);
    }

    public void Hide()
    {
        if (dialogueContent != null)
            dialogueContent.SetActive(false);
    }
}