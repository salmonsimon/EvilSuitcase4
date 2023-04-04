using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = GameManager.instance.GetSFXManager().GetSFXVolume();
        audioSource.playOnAwake = false;

        audioSource.spatialBlend = .8f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 50f;
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomClip = Random.Range(0, audioClips.Count);

        audioSource.PlayOneShot(audioClips[randomClip]);
    }
}
