using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class HurtGlobalVolumeAnimator : MonoBehaviour
{
    private Volume volume;
    private ColorCurves colorCurves;

    [SerializeField] private float stabilizingTime = 1f;

    private bool isEnabled = false;
    public bool IsEnabled { get { return isEnabled; } }

    private bool isEnabling = false;
    public bool IsEnabling { get { return isEnabling; } }

    private void Awake()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet<ColorCurves>(out colorCurves);
    }

    public void Enable()
    {
        if (!isEnabled)
        {
            StopAllCoroutines();

            StartCoroutine(EnableCoroutine());
        }
    }

    private IEnumerator EnableCoroutine()
    {
        isEnabling = true;

        float elapsedTime = 0f;

        Keyframe saturation = colorCurves.hueVsSat.value[0];
        float initialSaturation = saturation.value;

        float initialWeight = volume.weight;

        while (elapsedTime < stabilizingTime)
        {
            elapsedTime += Time.deltaTime;
            float percentualElapsedTime = elapsedTime / stabilizingTime;

            float newSaturationValue = Mathf.Lerp(initialSaturation, 0f, percentualElapsedTime);
            float newVolumeWeight = Mathf.Lerp(initialWeight, 1, percentualElapsedTime);

            saturation.value = newSaturationValue;

            colorCurves.hueVsSat.value.MoveKey(0, saturation);

            volume.weight = newVolumeWeight;

            yield return null;
        }

        saturation.value = 0f;

        colorCurves.hueVsSat.value.MoveKey(0, saturation);

        volume.weight = 1;

        isEnabled = true;
        isEnabling = false;
    }

    public void Disable()
    {
        if (GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.IsEnabled) // && this.IsEnabled)
        {
            volume.weight = 0;

            Keyframe saturation = colorCurves.hueVsSat.value[0];
            saturation.value = 0.5f;

            colorCurves.hueVsSat.value.MoveKey(0, saturation);

            isEnabled = false;
        }
        else if (isEnabled || IsEnabling)
        {
            StartCoroutine(DisableCoroutine());
        }
    }

    private IEnumerator DisableCoroutine()
    {
        while (IsEnabling)
            yield return null;

        float elapsedTime = 0f;

        Keyframe saturation = colorCurves.hueVsSat.value[0];
        float initialSaturation = saturation.value;

        float initialWeight = volume.weight;

        while (elapsedTime < stabilizingTime)
        {
            elapsedTime += Time.deltaTime;
            float percentualElapsedTime = elapsedTime / stabilizingTime;

            float newSaturationValue = Mathf.Lerp(initialSaturation, 0.5f, percentualElapsedTime);
            float newVolumeWeight = Mathf.Lerp(initialWeight, 0, percentualElapsedTime);

            saturation.value = newSaturationValue;

            colorCurves.hueVsSat.value.MoveKey(0, saturation);

            volume.weight = newVolumeWeight;

            yield return null;
        }

        saturation.value = 0.5f;

        colorCurves.hueVsSat.value.MoveKey(0, saturation);

        volume.weight = 0;

        isEnabled = false;
    }
}
