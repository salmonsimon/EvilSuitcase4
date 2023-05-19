using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapons/Audio Configuration", fileName = "AudioConfiguration", order = 4)]
public class MeleeWeaponAudioConfigurationScriptableObject : ScriptableObject
{
    [Range(0, 1f)]
    [SerializeField] private float volume = 1f;
    public float Volume { get { return volume; } }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] attackClips;
    public AudioClip[] AttackClips { get { return attackClips; } }

    [SerializeField] private AudioClip breakClip;
    public AudioClip BreakClip { get { return breakClip; } }

    public AudioClip GetRandomAttackClip()
    {
        if (AttackClips.Length == 0)
            return null;

        AudioClip attackClipToPlay = AttackClips[Random.Range(0, AttackClips.Length)];

        return attackClipToPlay;
    }
}
