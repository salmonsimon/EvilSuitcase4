using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Audio Configuration", fileName = "AudioConfiguration", order = 7)]
public class GunAudioConfigurationScriptableObject : ScriptableObject
{
    [Range(0, 1f)]
    [SerializeField] private float volume = 1f;
    public float Volume { get { return volume; } }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] fireClips;
    public AudioClip[] FireClips { get { return fireClips; } }

    [SerializeField] private AudioClip emptyClip;
    public AudioClip EmptyClip { get { return emptyClip; } }

    public AudioClip GetRandomShootingClip()
    {
        if (FireClips.Length == 0)
            return null;

        AudioClip fireClipToPlay = FireClips[Random.Range(0, FireClips.Length)];

        return fireClipToPlay;
    }
}
