using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Audio Configuration", fileName = "AudioConfiguration", order = 7)]
public class AudioConfigurationScriptableObject : ScriptableObject
{
    [Range(0, 1f)]
    public float Volume = 1f;

    [Header("Audio Clips")]
    public AudioClip[] FireClips;
    public AudioClip EmptyClip;

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
