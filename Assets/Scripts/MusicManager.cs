using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip ambientMusic;
    public AudioClip bossMusic;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        PlayAmbientMusic();
    }

    public void PlayAmbientMusic()
    {
        audioSource.clip = ambientMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayBossMusic()
    {
        audioSource.Stop();

        audioSource.clip = bossMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}