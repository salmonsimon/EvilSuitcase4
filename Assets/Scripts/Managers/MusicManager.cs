using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float musicVolume = 1f;

    private float playDelay = Config.BIG_DELAY;

    private float fadeDuration = Config.BIG_DELAY * 2;

    [Header("Single Tracks")]
    [Space(2)]

    [SerializeField] private AudioClip mainMenuMusic;

    [Header("Gameplay Tracks")]
    [Space(2)]

    [SerializeField] private List<AudioClip> gameplayMusic;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateVolume(Settings.Instance.musicVolume);
    }

    public void UpdateVolume(float value)
    {
        musicVolume = value;

        audioSource.volume = musicVolume;
        Settings.Instance.musicVolume = musicVolume;

        Settings.Save();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Config.MAIN_MENU_SCENE_NAME)
            PlayMusic(scene.name);
    }

    public void PlayMusic(string sceneName)
    {
        StopMusic();

        switch (sceneName)
        {
            case Config.ROOFTOP_SCENE_NAME:

                audioSource.loop = false;

                StartCoroutine(PlayGameplayMusic());

                break;
        }
    }

    public void PlayMainMenuMusic()
    {
        StopMusic();

        audioSource.loop = true;
        audioSource.clip = mainMenuMusic;

        StartCoroutine(WaitAndPlay(playDelay * 2 + Config.LARGE_DELAY));
    }

    public void StopMusic()
    {
        StopAllCoroutines();

        WaitAndStop(fadeDuration + Config.SMALL_DELAY);
    }

    private IEnumerator PlayGameplayMusic()
    {
        float nextSongRandomDelay = playDelay;

        while (true)
        {
            gameplayMusic.Shuffle();

            foreach (AudioClip song in gameplayMusic)
            {
                audioSource.clip = song;

                StartCoroutine(WaitAndPlayRealtime(nextSongRandomDelay));

                yield return new WaitForSecondsRealtime(song.length - 1);

                StartCoroutine(FadeOut());

                yield return new WaitForSecondsRealtime(1 + nextSongRandomDelay);

                nextSongRandomDelay = Random.Range(playDelay, playDelay * 4);
            }
        }
    }

    private IEnumerator WaitAndStop(float duration)
    {
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(duration);

        audioSource.Stop();
    }

    private IEnumerator WaitAndPlay(float duration)
    {
        yield return new WaitForSeconds(duration);

        audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator WaitAndPlayRealtime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;

        audioSource.volume = 0;

        float startingVolume = audioSource.volume;
        float targetVolume = Settings.Instance.musicVolume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            audioSource.volume = Mathf.Lerp(startingVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;

        audioSource.volume = Settings.Instance.musicVolume;

        float startingVolume = audioSource.volume;
        float targetVolume = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.fixedDeltaTime;
            audioSource.volume = Mathf.Lerp(startingVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
