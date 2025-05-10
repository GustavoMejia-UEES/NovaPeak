using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayCutsceneOnTrigger : MonoBehaviour
{
    public GameObject cutsceneCanvas; // Canvas con el RawImage (así puedes activar/desactivar todo)
    public VideoPlayer videoPlayer;   // El VideoPlayer que tiene la RenderTexture asignada
    public string nextSceneName;      // Nombre de la siguiente escena

    private bool hasPlayed = false;

    void Start()
    {
        // Asegúrate de que el Canvas esté desactivado al inicio
        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;
            if (cutsceneCanvas != null)
                cutsceneCanvas.SetActive(true); // Muestra el Canvas con el RawImage
            if (videoPlayer != null)
            {
                videoPlayer.Play();
                videoPlayer.loopPointReached += OnVideoEnd;
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}