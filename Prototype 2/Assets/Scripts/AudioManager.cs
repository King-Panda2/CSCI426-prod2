using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private float fadeDuration = 1.5f; // Duration of fade effect

    // Sound effects
    public AudioClip death;
    public AudioClip jump;
    public AudioClip trumpets;
    public AudioClip clapping;


    public AudioClip winTrack;

    public AudioClip background1;
    public AudioClip background2;
    public AudioClip background3;
    public AudioClip background4;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null || musicClip == null) return;
        if (musicSource.clip == musicClip && musicSource.isPlaying) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeMusic(musicClip));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // Fade Out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Change Music
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade In
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    // Play sound effects
    public void PlaySFX(AudioClip sfxClip)
    {
        if (SFXSource != null && sfxClip != null)
        {
            SFXSource.PlayOneShot(sfxClip);
        }
    }
}
