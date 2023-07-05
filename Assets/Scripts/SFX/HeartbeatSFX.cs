using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatSFX : AmbienceSFX
{
    [SerializeField] private AudioClip heartbeatSingleSFX;

    protected override void Start()
    {
        base.Start();

        audioSource.clip = heartbeatSingleSFX;
    }

    public void Heartbeat()
    {
        StartCoroutine(HeartbeatCoroutine());
    }

    private IEnumerator HeartbeatCoroutine()
    {
        audioSource.pitch = 1;

        while (true)
        {
            audioSource.Play();

            while (audioSource.isPlaying)
                yield return null;

            yield return null;

            float heartbeatDelay = .8f - (audioSource.pitch - 1f);

            yield return new WaitForSecondsRealtime(heartbeatDelay);
        }
    }

    public void OnDamaged()
    {
        StartCoroutine(OnDamagedCoroutine());
    } 

    private IEnumerator OnDamagedCoroutine()
    {
        while (audioSource.isPlaying)
            yield return null;

        yield return null;

        audioSource.pitch += .1f;
    }

    public void DyingHeartbeat()
    {
        StopCoroutine(HeartbeatCoroutine());

        StartCoroutine(DyingHeartbeatCoroutine());
    }

    private IEnumerator DyingHeartbeatCoroutine()
    {
        while (audioSource.isPlaying)
            yield return null;

        float heartbeatDelay = .8f - (audioSource.pitch - 1f);

        if (heartbeatDelay < .4f)
            heartbeatDelay = .4f;

        yield return new WaitForSecondsRealtime(heartbeatDelay);

        while (audioSource.pitch > .5)
        {
            audioSource.Play();

            while (audioSource.isPlaying)
                yield return null;

            audioSource.pitch -= .1f;

            heartbeatDelay = .8f - (audioSource.pitch - 1f);

            if (heartbeatDelay < .4f)
                heartbeatDelay = .4f;

            yield return new WaitForSecondsRealtime(heartbeatDelay);
        }

        StopSFX();
    }

    public void StopSFX()
    {
        StopAllCoroutines();

        audioSource.Stop();
    }

    protected override void OnVolumeChange()
    {
        audioSource.volume = Settings.Instance.SFXVolume;
    }
}
