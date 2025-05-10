using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class BossWatcherAndCinematic : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject boss; // Asigna el objeto del jefe en el Inspector
    public VideoPlayer videoPlayer; // Asigna el VideoPlayer para la cinemática final
    public Canvas videoCanvas; // Asigna el Canvas que contiene el video

    [Header("Cinemática")]
    public float timeBetweenRepeats = 0.5f; // Tiempo entre repeticiones del video
    public int repeatCount = 3; // Número de veces que se repetirá el video

    private bool cinematicStarted = false;
    private int currentRepeat = 0;

    void Start()
    {
        // Desactivar el Canvas al inicio
        if (videoCanvas != null)
            videoCanvas.gameObject.SetActive(false);

        // Configurar el VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, videoPlayer.GetComponent<AudioSource>());
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetDirectAudioMute(0, false);
        }
    }

    void Update()
    {
        if (!cinematicStarted && boss == null)
        {
            cinematicStarted = true;
            StartFinalCinematic();
        }
    }

    void StartFinalCinematic()
    {
        currentRepeat = 0;

        // Detener la música actual si hay MusicManager
        var musicManager = FindFirstObjectByType<MusicManager>();
        if (musicManager != null)
            musicManager.StopMusic();

        PlayVideo();
    }

    void PlayVideo()
    {
        if (videoPlayer != null && videoCanvas != null)
        {
            videoCanvas.gameObject.SetActive(true);
            videoPlayer.Play();
            Debug.Log($"Reproduciendo video final (Repetición {currentRepeat + 1}/{repeatCount})");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        currentRepeat++;

        if (currentRepeat < repeatCount)
        {
            StartCoroutine(RepeatVideoAfterDelay());
        }
        else
        {
            StartCoroutine(EndGame());
        }
    }

    IEnumerator RepeatVideoAfterDelay()
    {
        if (videoCanvas != null)
            videoCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(timeBetweenRepeats);
        PlayVideo();
    }

    IEnumerator EndGame()
    {
        Debug.Log("Finalizando juego...");

        if (videoCanvas != null)
            videoCanvas.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;
    }
}