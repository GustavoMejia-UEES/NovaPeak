using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip musica1; // Música de fondo del nivel
    public AudioClip musica2; // Música del boss

    void Start()
    {
        // Al iniciar, suena la música 1
        if (audioSource != null && musica1 != null)
        {
            audioSource.clip = musica1;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Llama a este método desde el script del boss/cinemática
    public void PlayBossMusic()
    {
        if (audioSource != null && musica2 != null)
        {
            audioSource.Stop();
            audioSource.clip = musica2;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Método para detener la música actual
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}