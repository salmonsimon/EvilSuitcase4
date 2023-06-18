using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSFX : SFX
{
    [SerializeField] private List<AudioClip> screamAudioClips;
    [SerializeField] private List<AudioClip> attackAudioClips;
    [SerializeField] private List<AudioClip> shortSoundAudioClips;
    [SerializeField] private List<AudioClip> hurtAudioClips;
    [SerializeField] private List<AudioClip> deathAudioClips;
    [SerializeField] private List<AudioClip> stepAudioClips;
    [SerializeField] private List<AudioClip> eatingAudioClips;
    [SerializeField] private List<AudioClip> hitImpactAudioClip;

    private float timeUntilNextShortSound = -1f;
    private float shortSoundMaxDelay = 2f;

    private float timeUntilNexEatingSound = -1f;

    private void Update()
    {
        timeUntilNextShortSound -= Time.deltaTime;
        timeUntilNexEatingSound -= Time.deltaTime;
    }

    public void PlayRandomScreamAudioClip()
    {
        PlayRandomAudioClip(screamAudioClips);
    }

    public void PlayRandomAttackAudioClip()
    {
        PlayRandomAudioClip(attackAudioClips);
    }

    public void PlayRandomShortSoundAudioClip()
    {
        if (timeUntilNextShortSound < 0)
        {
            int randomClipIndex = UnityEngine.Random.Range(0, shortSoundAudioClips.Count);

            PlayAudioClip(shortSoundAudioClips[randomClipIndex]);

            timeUntilNextShortSound = UnityEngine.Random.Range(shortSoundAudioClips[randomClipIndex].length, shortSoundAudioClips[randomClipIndex].length + shortSoundMaxDelay);
        }
    }

    public void PlayRandomHurtAudioClip()
    {
        PlayRandomAudioClip(hurtAudioClips);
    }

    public void PlayRandomDeathAudioClip()
    {
        PlayRandomAudioClip(deathAudioClips);
    }
    public void PlayRandomStepAudioClip()
    {
        int randomStepAudioClipIndex = UnityEngine.Random.Range(0, stepAudioClips.Count);
        AudioClip randomStepAudioClip = stepAudioClips[randomStepAudioClipIndex];

        AudioSource.PlayClipAtPoint(randomStepAudioClip, transform.position, GameManager.instance.GetSFXManager().GetSFXVolume());
    }

    public void PlayRandomEatingAudioClip()
    {
        if (timeUntilNexEatingSound < 0)
        {
            int randomEatingAudioClipIndex = UnityEngine.Random.Range(0, eatingAudioClips.Count);

            AudioClip randomEatingAudioClip = eatingAudioClips[randomEatingAudioClipIndex];

            PlayAudioClip(randomEatingAudioClip);

            timeUntilNexEatingSound = UnityEngine.Random.Range(randomEatingAudioClip.length, randomEatingAudioClip.length + 1f);
        }
    }

    public void PlayRandomHitImpactAudioClip()
    {
        PlayRandomAudioClip(hitImpactAudioClip);
    }
}
