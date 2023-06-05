using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CinemachineShake : MonoBehaviour
{
    private List<CinemachineVirtualCamera> cinemachineVirtualCameraList = new List<CinemachineVirtualCamera>();
    private List<CinemachineBasicMultiChannelPerlin> cinemachineBasicMultiChannelPerlinList = new List<CinemachineBasicMultiChannelPerlin>();

    private NoiseSettings noiseSettings;

    private float startingIntensity;

    private float shakeTimer;
    private float shakeTimerTotal;

    private bool shakingUI;

    private void Awake()
    {
        noiseSettings = Resources.Load(Config.SHAKE_FILE) as NoiseSettings;

        cinemachineVirtualCameraList = FindObjectsOfType<CinemachineVirtualCamera>().ToList();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetVirtualCameras(cinemachineVirtualCameraList);
    }

    public void SetVirtualCameras(List<CinemachineVirtualCamera> virtualCameras)
    {
        foreach (CinemachineVirtualCamera virtualCamera in virtualCameras)
        {
                if (virtualCamera.TryGetComponent(out CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin))
                {
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
                    cinemachineBasicMultiChannelPerlin.m_NoiseProfile = noiseSettings;

                    cinemachineBasicMultiChannelPerlinList.Add(cinemachineBasicMultiChannelPerlin);
                }
                else
                {
                    CinemachineBasicMultiChannelPerlin perlin = virtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                    perlin.m_AmplitudeGain = 0;
                    perlin.m_NoiseProfile = noiseSettings;

                    cinemachineBasicMultiChannelPerlinList.Add(perlin);
                }
        }
    }

    public void ShakeCameras(float intensity, float time)
    {
        foreach (CinemachineBasicMultiChannelPerlin perlin in cinemachineBasicMultiChannelPerlinList)
            perlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            foreach (CinemachineBasicMultiChannelPerlin perlin in cinemachineBasicMultiChannelPerlinList)
                perlin.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakeTimer / shakeTimerTotal);
        }
    }
}
