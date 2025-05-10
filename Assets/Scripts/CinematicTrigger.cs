using UnityEngine;
using UnityEngine.Video;

public class CinematicTrigger : MonoBehaviour
{
    public GameObject[] enemies; // Asigna todos los enemigos en el Inspector
    public GameObject player;    // Asigna el Player
    public VideoPlayer videoPlayer; // Asigna el VideoPlayer (en un Canvas o RawImage)
    public Light directionalLight;  // Asigna la Directional Light
    public Color newLightColor = Color.red;
    public Camera mainCamera;       // Asigna la Main Camera
    public float newFOV = 40f;
    public float fovTransitionTime = 1f;

    private bool triggered = false;
    private Canvas videoCanvas; // Referencia al Canvas que contiene el video
    private MusicManager musicManager; // Referencia al MusicManager

    void Start()
    {
        // Obtener referencia al Canvas
        if (videoPlayer != null)
        {
            videoCanvas = videoPlayer.GetComponentInParent<Canvas>();
            if (videoCanvas != null)
            {
                // Desactivar el Canvas al inicio
                videoCanvas.gameObject.SetActive(false);
                Debug.Log("Canvas desactivado al inicio");
            }
            else
            {
                Debug.LogError("No se encontró el Canvas que contiene el VideoPlayer!");
            }
        }

        // Obtener referencia al MusicManager
        musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager == null)
        {
            Debug.LogError("No se encontró el MusicManager en la escena!");
        }

        // Configurar el VideoPlayer para reproducir audio
        if (videoPlayer != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, videoPlayer.GetComponent<AudioSource>());
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetDirectAudioMute(0, false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Trigger activado - Iniciando cinemática");

            // 1. Detener enemigos (movimiento y daño)
            foreach (var enemy in enemies)
            {
                var ai = enemy.GetComponent<EnemyAI>();
                if (ai != null) ai.enabled = false;
                var anim = enemy.GetComponent<Animator>();
                if (anim != null) anim.enabled = false;
                var health = enemy.GetComponent<EnemyHealth>();
                if (health != null) health.enabled = false;
            }

            // 2. Detener jugador (movimiento y daño)
            var playerMotor = player.GetComponent<PlayerMotor>();
            if (playerMotor != null) playerMotor.enabled = false;
            var playerAnim = player.GetComponent<Animator>();
            if (playerAnim != null) playerAnim.enabled = false;
            var playerCombat = player.GetComponent<PlayerCombat>();
            if (playerCombat != null) playerCombat.enabled = false;
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.enabled = false;

            // 3. Detener la música del nivel
            if (musicManager != null)
            {
                musicManager.StopMusic();
                Debug.Log("Música del nivel detenida");
            }

            // 4. Activar Canvas y reproducir video
            if (videoPlayer != null && videoCanvas != null)
            {
                videoCanvas.gameObject.SetActive(true);
                videoPlayer.loopPointReached += OnVideoEnd;
                videoPlayer.Play();
                Debug.Log("Canvas activado y video iniciado");
            }
            else
            {
                Debug.LogError("VideoPlayer o Canvas no asignados!");
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video terminado - Reactivando escena");
        
        // 1. Reactivar enemigos
        foreach (var enemy in enemies)
        {
            var ai = enemy.GetComponent<EnemyAI>();
            if (ai != null) ai.enabled = true;
            var anim = enemy.GetComponent<Animator>();
            if (anim != null) anim.enabled = true;
            var health = enemy.GetComponent<EnemyHealth>();
            if (health != null) health.enabled = true;
        }

        // 2. Reactivar jugador
        var playerMotor = player.GetComponent<PlayerMotor>();
        if (playerMotor != null) playerMotor.enabled = true;
        var playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null) playerAnim.enabled = true;
        var playerCombat = player.GetComponent<PlayerCombat>();
        if (playerCombat != null) playerCombat.enabled = true;
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.enabled = true;

        // 3. Cambiar música usando MusicManager
        if (musicManager != null)
        {
            musicManager.PlayBossMusic();
            Debug.Log("Música de boss iniciada");
        }
        else
        {
            Debug.LogError("MusicManager no encontrado!");
        }

        // 4. Cambiar color de la luz
        if (directionalLight != null)
            directionalLight.color = newLightColor;

        // 5. Cambiar FOV de la cámara (con transición)
        if (mainCamera != null)
            StartCoroutine(ChangeFOV(mainCamera, newFOV, fovTransitionTime));

        // 6. Desactivar el Canvas y limpiar el VideoPlayer
        if (videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(false);
            Debug.Log("Canvas desactivado");
        }
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd; // Desuscribir el evento
        }

        Debug.Log("Cinemática completada - Todo reactivado");
    }

    System.Collections.IEnumerator ChangeFOV(Camera cam, float targetFOV, float duration)
    {
        float startFOV = cam.fieldOfView;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = targetFOV;
    }
}