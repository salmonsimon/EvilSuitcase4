using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Audio Configuration", fileName = "AudioConfiguration", order = 7)]
public class AudioConfigurationScriptableObject : ScriptableObject
{
    [Range(0, 1f)]
    [SerializeField] private float volume = 1f;
    public float Volume { get { return volume; } }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] fireClips;
    public AudioClip[] FireClips { get { return fireClips; } }

    [SerializeField] private AudioClip emptyClip;
    public AudioClip EmptyClip { get { return emptyClip; } }

    public void PlayShootingClip()
    {
        if (FireClips.Length == 0)
            return;

        AudioClip fireClipToPlay = FireClips[Random.Range(0, FireClips.Length)];

        GameManager.instance.GetSFXManager().PlaySound(fireClipToPlay);
    }

    public void PlayEmptyClip()
    {
        if (!EmptyClip)
            return;

        GameManager.instance.GetSFXManager().PlaySound(EmptyClip);
    }
}
