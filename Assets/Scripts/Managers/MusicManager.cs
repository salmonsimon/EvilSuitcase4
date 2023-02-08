using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float musicVolume = 1f;

    private float playDelay = Config.BIG_DELAY;

    [Header("Single Tracks")]
    [Space(2)]

    [SerializeField] private AudioClip mainMusic;

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
        audioSource.loop = true;
        audioSource.clip = mainMusic;
        StartCoroutine(WaitAndPlay(playDelay));
    }
    private void OnDisable()
    {
        StopMusic();
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }

    private IEnumerator WaitAndPlay(float duration)
    {
        yield return new WaitForSeconds(duration);

        audioSource.Play();
    }

    public string GetCurrentAudioClipName()
    {
        if (audioSource.isPlaying)
            return audioSource.clip.name;

        return "None";
    }
}
