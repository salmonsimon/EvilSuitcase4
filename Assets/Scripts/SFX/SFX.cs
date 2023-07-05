using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(AudioSource))]
public class SFX : MonoBehaviour
{
    protected AudioSource audioSource;

    [SerializeField] private bool isUI = true;

    bool initialized = false;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SFXManager sfxManager = GameManager.instance.GetSFXManager();

        audioSource.volume = sfxManager.GetSFXVolume();
        audioSource.playOnAwake = false;

        if (!isUI)
        {
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 3f;
            audioSource.maxDistance = 200f;
        }

        GameManager.instance.GetPauseMenuUI().OnPause += OnPause;
        GameManager.instance.GetPauseMenuUI().OnResume += OnResume;

        sfxManager.OnVolumeChange += OnVolumeChange;

        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
        {
            GameManager.instance.GetPauseMenuUI().OnPause += OnPause;
            GameManager.instance.GetPauseMenuUI().OnResume += OnResume;

            GameManager.instance.GetSFXManager().OnVolumeChange += OnVolumeChange;
        }
    }

    private void OnDisable()
    {
        if (initialized)
        {
            GameManager.instance.GetPauseMenuUI().OnPause -= OnPause;
            GameManager.instance.GetPauseMenuUI().OnResume -= OnResume;

            GameManager.instance.GetSFXManager().OnVolumeChange -= OnVolumeChange;
        }   
    }

    protected virtual void OnVolumeChange()
    {
        if (audioSource.isPlaying)
            audioSource.volume = Settings.Instance.SFXVolume;
    }

    protected virtual void OnPause()
    {
        audioSource.Pause();
    }

    protected virtual void OnResume()
    {
        audioSource.UnPause();
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        if (isUI)
            audioSource.PlayOneShot(audioClip);
        else
        {
            audioSource.Stop();

            audioSource.clip = audioClip;

            audioSource.Play();
        }
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomClip = Random.Range(0, audioClips.Count);

        if (isUI)
            audioSource.PlayOneShot(audioClips[randomClip]);
        else
        {
            audioSource.Stop();

            audioSource.clip = audioClips[randomClip];

            audioSource.Play();
        }
    }
}
