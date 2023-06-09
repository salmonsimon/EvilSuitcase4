using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float sfxVolume = 1f;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip backSFX;
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip menuChangeSFX;
    [SerializeField] private AudioClip welcomingScreenSFX;
    [SerializeField] private AudioClip evilLaughSFX;
    [SerializeField] private AudioClip pauseSFX;
    [SerializeField] private AudioClip resumeSFX;
    [SerializeField] private AudioClip autoSortSFX;

    [SerializeField] private List<AudioClip> pickupSFX;
    [SerializeField] private List<AudioClip> dropSFX;

    [SerializeField] private AudioClip rotateSFX;
    [SerializeField] private AudioClip discardSFX;

    [SerializeField] private AudioClip equipSFX;

    [SerializeField] private List<AudioClip> countdownSFX;

    [SerializeField] private AudioClip waveStartSFX;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip transitionStartSFX;
    [SerializeField] private AudioClip transitionEndSFX;
    [SerializeField] private AudioClip bloodSplatterSFX;
    [SerializeField] private AudioClip heartbeatSFX;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        UpdateVolume(Settings.Instance.SFXVolume);
    }

    public void UpdateVolume(float value)
    {
        sfxVolume = value;

        audioSource.volume = sfxVolume;
        Settings.Instance.SFXVolume = sfxVolume;

        Settings.Save();
    }

    public void PlaySound(string str)
    {
        switch (str)
        {
            case Config.CLICK_SFX:
                audioSource.PlayOneShot(clickSFX);
                break;

            case Config.BACK_SFX:
                audioSource.PlayOneShot(backSFX);
                break;

            case Config.HOVER_SFX:
                audioSource.PlayOneShot(hoverSFX);
                break;

            case Config.MENU_CHANGE_SFX:
                audioSource.PlayOneShot(menuChangeSFX);
                break;

            case Config.WELCOMING_SCREEN_SFX:
                audioSource.PlayOneShot(welcomingScreenSFX);
                break;

            case Config.EVIL_LAUGH_SFX:
                audioSource.PlayOneShot(evilLaughSFX);
                break;

            case Config.PAUSE_SFX:
                audioSource.PlayOneShot(pauseSFX);
                break;

            case Config.RESUME_SFX:
                audioSource.PlayOneShot(resumeSFX);
                break;

            case Config.AUTO_SORT_SFX:
                audioSource.PlayOneShot(autoSortSFX);
                break;

            case Config.PICKUP_SFX:
                PlayRandomAudioClip(pickupSFX);
                break;

            case Config.DROP_SFX:
                PlayRandomAudioClip(dropSFX);
                break;

            case Config.ROTATE_SFX:
                audioSource.PlayOneShot(rotateSFX);
                break;

            case Config.DISCARD_SFX:
                audioSource.PlayOneShot(discardSFX);
                break;

            case Config.EQUIP_SFX:
                audioSource.PlayOneShot(equipSFX);
                break;

            case Config.COUNTDOWN_SFX:
                PlayRandomAudioClip(countdownSFX);
                break;

            case Config.WAVE_START_SFX:
                audioSource.PlayOneShot(waveStartSFX);
                break;

            case Config.GAME_OVER_SFX:
                audioSource.PlayOneShot(gameOverSFX);
                break;

            case Config.TRANSITION_START_SFX:
                audioSource.PlayOneShot(transitionStartSFX);
                break;

            case Config.TRANSITION_END_SFX:
                audioSource.PlayOneShot(transitionEndSFX);
                break;

            case Config.BLOOD_SPLATTER_SFX:
                audioSource.PlayOneShot(bloodSplatterSFX);
                break;

            case Config.HEARTBEAT_SFX:
                audioSource.PlayOneShot(heartbeatSFX);
                break;
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlaySound(AudioClip audioClip, float volume)
    {
        audioSource.PlayOneShot(audioClip, volume*sfxVolume);
    }

    public void PlayRandomAudioClip(List<AudioClip> audioClips)
    {
        int randomClip = Random.Range(0, audioClips.Count);

        PlaySound(audioClips[randomClip]);
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}
