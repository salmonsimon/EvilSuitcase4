using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSFX : SFX
{
    [SerializeField] private List<AudioClip> pumpReloadAudioClips;
    [SerializeField] private List<AudioClip> bulletReloadAudioClips;

    public void PlayRandomPumpReloadAudioClip()
    {
        PlayRandomAudioClip(pumpReloadAudioClips);
    }

    public void PlayRandomBulletReloadAudioClip()
    {
        PlayRandomAudioClip(bulletReloadAudioClips);
    }
}
