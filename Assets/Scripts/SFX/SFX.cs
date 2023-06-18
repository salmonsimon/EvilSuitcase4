using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private bool isUI = true; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = GameManager.instance.GetSFXManager().GetSFXVolume();
        audioSource.playOnAwake = false;

        audioSource.spatialBlend = .8f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 50f;

        GameManager.instance.GetPauseMenuUI().OnPause += OnPause;
        GameManager.instance.GetPauseMenuUI().OnResume += OnResume;
    }

    private void OnPause()
    {
        audioSource.Pause();
    }

    private void OnResume()
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
