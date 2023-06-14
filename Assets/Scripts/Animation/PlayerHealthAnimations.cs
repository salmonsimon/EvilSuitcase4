using Cinemachine;
using DG.Tweening;
using GLTF.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthAnimations : MonoBehaviour
{
    [Header("Player Spine Reference")]
    [SerializeField] private Transform playerSpine;
    public Transform PlayerSpine { get { return playerSpine; } }

    [Header("Animation References")]
    [SerializeField] private List<AnimationClip> hurtAnimations;
    [SerializeField] private List<AnimationClip> deathAnimations;

    [Header("Audio References")]
    [SerializeField] private List<AudioClip> hurtAudioClips;
    [SerializeField] private List<AudioClip> bluntWeaponHitAudioClips;
    [SerializeField] private List<AudioClip> deathAudioClips;

    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera playerDeadCamera;
    [SerializeField] private Transform playerDeadCameraRootTransform;

    [Header("Dead Camera Configuration")]
    [SerializeField] private float deadCameraRootDefaultHeight = 2.5f;
    [SerializeField] private float deadCameraMaxDistanceTraveled = 5f;
    [SerializeField] private float deadCameraTravelSpeed = .5f;
    [SerializeField] private float deadCameraRotationSpeed = 5f;

    [Header("Other Configurations")]
    [SerializeField] private float criticalHealthThreshold = .35f;

    private float deadCameraCurrentDistanceTraveled = 0f;

    private Animator animator;
    private ThirdPersonShooterController playerThirdPersonShooterController;
    private HealthManager playerHealthManager;
    private SFX sfx;

    private List<Collider> hurtColliders = new List<Collider>();

    private bool isOnHurtAnimation = false;
    public bool IsOnHurtAnimation { get { return isOnHurtAnimation; } }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerThirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        playerHealthManager = GetComponent<HealthManager>();
        sfx = GetComponent<SFX>();

        playerHealthManager.OnDamaged += Damaged;
        playerHealthManager.OnRecover += Recover;
        playerHealthManager.OnDeath += Death;
        playerHealthManager.OnRevival += Revival;

        playerDeadCamera.gameObject.SetActive(false);


        foreach (HumanoidHurtGeometry humanoidHurtGeometry in
                 GameManager.instance.GetPlayer().GetComponentsInChildren<HumanoidHurtGeometry>())
        {
            hurtColliders.Add(humanoidHurtGeometry.GetComponent<Collider>());
        }
    }

    private void Damaged()
    {
        if (playerHealthManager.IsAlive && playerThirdPersonShooterController.CanTriggerHurtAnimation())
            PlayRandomHurtAnimation();

        float currentHitPoints = playerHealthManager.CurrentHitPoints;
        float maxHitPoints = playerHealthManager.MaxHitPoints;
        float hitPointsPercentage = currentHitPoints / maxHitPoints;

        if (hitPointsPercentage < criticalHealthThreshold && !GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.IsEnabled)
        {
            //animator.SetBool("IsHurt", true);
            GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.Enable();
        }
            
    }

    private void Recover()
    {
        float currentHitPoints = playerHealthManager.CurrentHitPoints;
        float maxHitPoints = playerHealthManager.MaxHitPoints;
        float hitPointsPercentage = currentHitPoints / maxHitPoints;

        if (hitPointsPercentage > criticalHealthThreshold && GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.IsEnabled)
        {
            //animator.SetBool("IsHurt", false);
            GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.Disable();
        }
            
    }

    private void Death()
    {
        ActivateHurtColliders();
        PlayRandomDeathAnimation();
    }

    private void Revival()
    {
        GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.Disable();
        GameManager.instance.GetPlayerHealthUI().HurtGlobalVolume.Disable();

        DisableDeadCamera();

        isOnHurtAnimation = false;
    }

    public void PlayRandomHurtAnimation()
    {
        if (isOnHurtAnimation)
            return;

        int hurtAnimationIndex = Random.Range(0, hurtAnimations.Count);
        AnimationClip hurtAnimationToPlay = hurtAnimations[hurtAnimationIndex];

        isOnHurtAnimation = true;
        StartCoroutine(PlayClip(Animator.StringToHash(hurtAnimationToPlay.name), 0f));

        if (!GameManager.instance.GetPlayerHealthUI().CriticalHealthGlobalVolume.IsEnabled)
        {
            GameManager.instance.GetPlayerHealthUI().HurtGlobalVolume.Enable();

            StartCoroutine(WaitToDisableHurtGlobalVolume());
        }
    }

    private IEnumerator WaitToDisableHurtGlobalVolume()
    {
        while (IsOnHurtAnimation) yield return null;

        GameManager.instance.GetPlayerHealthUI().HurtGlobalVolume.Disable();
    }

    private void ActivateHurtColliders()
    {
        foreach (Collider collider in hurtColliders)
            collider.isTrigger = false;
    }

    private void PlayRandomDeathAnimation()
    {
        StopAllCoroutines();

        if (isOnHurtAnimation)
        {
            isOnHurtAnimation = false;
            GameManager.instance.GetPlayerHealthUI().HurtGlobalVolume.Disable();
        }

        int deathAnimationIndex = Random.Range(0, deathAnimations.Count);
        AnimationClip deathAnimationToPlay = deathAnimations[deathAnimationIndex];

        StartCoroutine(PlayClip(Animator.StringToHash(deathAnimationToPlay.name), 0f));

        StartCoroutine(DeadCameraRocketMovement());
        StartCoroutine(WaitAndOpenGameOverPanel(5f));
    }

    private IEnumerator WaitAndOpenGameOverPanel(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameManager.instance.GetPlayerHealthUI().GameOver();
    }

    private IEnumerator DeadCameraRocketMovement()
    {
        GameObject.FindGameObjectWithTag(Config.MAIN_CAMERA_TAG).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 2f;

        yield return null;

        Vector3 angles = playerDeadCamera.transform.rotation.eulerAngles;

        bool rotateClockwise = playerDeadCamera.transform.rotation.y > 0;

        playerDeadCamera.Priority = 30;
        playerDeadCamera.gameObject.SetActive(true);

        while (deadCameraCurrentDistanceTraveled < deadCameraMaxDistanceTraveled)
        {
            float traveledDistance = Time.deltaTime * deadCameraTravelSpeed;
            deadCameraCurrentDistanceTraveled += traveledDistance;

            playerDeadCameraRootTransform.position += new Vector3(0, traveledDistance, 0);

            float rotation = Time.deltaTime * deadCameraRotationSpeed;
            rotation = rotateClockwise ? rotation : -rotation;

            angles.y += rotation;

            playerDeadCamera.transform.rotation = Quaternion.Euler(angles);

            yield return null;
        }

        while (true)
        {
            float rotation = Time.deltaTime * deadCameraRotationSpeed;
            rotation = rotateClockwise ? rotation : -rotation;

            angles.y += rotation;

            playerDeadCamera.transform.rotation = Quaternion.Euler(angles);

            yield return null;
        }
    }

    private void DisableDeadCamera()
    {
        GameObject.FindGameObjectWithTag(Config.MAIN_CAMERA_TAG).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = .1f;

        playerDeadCamera.Priority = 1;
        playerDeadCamera.gameObject.SetActive(false);

        playerDeadCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        playerDeadCameraRootTransform.localPosition = new Vector3(0f, deadCameraRootDefaultHeight, 0f);
    }

    public void FinishHurtAnimation()
    {
        isOnHurtAnimation = false;
    }

    protected IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        animator.Play(clipHash);
    }

    public void PlayRandomHurtAudioClip()
    {
        sfx.PlayRandomAudioClip(hurtAudioClips);
    }

    public void PlayRandomDeathAudioClip()
    {
        sfx.PlayRandomAudioClip(deathAudioClips);
    }
}
