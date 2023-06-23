using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CriticHealthGlobalVolumeAnimator : MonoBehaviour
{
    private Volume volume;
    private ColorCurves colorCurves;

    [SerializeField] private float stabilizingTime = 1f;

    private bool isEnabled = false;
    public bool IsEnabled { get { return isEnabled; } }

    private void Awake()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet<ColorCurves>(out colorCurves);
    }

    public void Enable()
    {
        StopAllCoroutines();

        StartCoroutine(EnableCoroutine());
    }

    private IEnumerator EnableCoroutine()
    {
        isEnabled = true;

        float elapsedTime = 0f;

        Keyframe saturation = colorCurves.hueVsSat.value[0];

        while (elapsedTime < stabilizingTime)
        {
            elapsedTime += Time.deltaTime;
            float percentualElapsedTime = elapsedTime / stabilizingTime;

            float newSaturationValue = Mathf.Lerp(.5f, 0f, percentualElapsedTime);
            float newVolumeWeight = Mathf.Lerp(0, 1, percentualElapsedTime);

            saturation.value = newSaturationValue;

            colorCurves.hueVsSat.value.MoveKey(0, saturation);

            volume.weight = newVolumeWeight;

            yield return null;
        }

        saturation.value = 0f;

        colorCurves.hueVsSat.value.MoveKey(0, saturation);

        volume.weight = 1;
    }

    public void Disable()
    {
        StopAllCoroutines();

        StartCoroutine(DisableCoroutine());
    }

    private IEnumerator DisableCoroutine()
    {
        isEnabled = false;

        float elapsedTime = 0f;

        Keyframe saturation = colorCurves.hueVsSat.value[0];

        while (elapsedTime < stabilizingTime)
        {
            elapsedTime += Time.deltaTime;
            float percentualElapsedTime = elapsedTime / stabilizingTime;

            float newSaturationValue = Mathf.Lerp(0f, 0.5f, percentualElapsedTime);
            float newVolumeWeight = Mathf.Lerp(1, 0, percentualElapsedTime);

            saturation.value = newSaturationValue;

            colorCurves.hueVsSat.value.MoveKey(0, saturation);

            volume.weight = newVolumeWeight;

            yield return null;
        }

        saturation.value = 0.5f;

        colorCurves.hueVsSat.value.MoveKey(0, saturation);

        volume.weight = 0;
    }
}
