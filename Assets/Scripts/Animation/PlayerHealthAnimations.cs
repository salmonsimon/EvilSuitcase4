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
    [SerializeField] private List<AudioClip> deathAudioClips;

    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera playerDeadCamera;
    [SerializeField] private Transform playerDeadCameraRootTransform;

    [Header("Dead Camera Configuration")]
    [SerializeField] private float deadCameraMaxDistanceTraveled = 5f;
    [SerializeField] private float deadCameraTravelSpeed = .5f;
    [SerializeField] private float deadCameraRotationSpeed = 5f;

    private float deadCameraCurrentDistanceTraveled = 0f;

    private Animator animator;
    private ThirdPersonShooterController playerThirdPersonShooterController;
    private HealthManager playerHealthManager;
    private SFX sfx;

    private bool isOnHurtAnimation = false;
    public bool IsOnHurtAnimation { get { return isOnHurtAnimation; } }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerThirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        playerHealthManager = GetComponent<HealthManager>();
        sfx = GetComponent<SFX>();

        playerHealthManager.OnDamaged += Damaged;
        playerHealthManager.OnDeath += Death;

        playerDeadCamera.gameObject.SetActive(false);
    }

    private void Damaged()
    {
        if (playerHealthManager.IsAlive && playerThirdPersonShooterController.CanTriggerHurtAnimation())
            PlayRandomHurtAnimation();
    }

    private void Death()
    {
        PlayRandomDeathAnimation();
    }

    public void PlayRandomHurtAnimation()
    {
        if (isOnHurtAnimation)
            return;

        int hurtAnimationIndex = Random.Range(0, hurtAnimations.Count);
        AnimationClip hurtAnimationToPlay = hurtAnimations[hurtAnimationIndex];

        isOnHurtAnimation = true;
        StartCoroutine(PlayClip(Animator.StringToHash(hurtAnimationToPlay.name), 0f));

        sfx.PlayRandomAudioClip(hurtAudioClips);
    }

    public void PlayRandomDeathAnimation()
    {
        StopAllCoroutines();
        isOnHurtAnimation = false;

        int deathAnimationIndex = Random.Range(0, deathAnimations.Count);
        AnimationClip deathAnimationToPlay = deathAnimations[deathAnimationIndex];

        StartCoroutine(PlayClip(Animator.StringToHash(deathAnimationToPlay.name), 0f));

        sfx.PlayRandomAudioClip(deathAudioClips);

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

    public void FinishHurtAnimation()
    {
        isOnHurtAnimation = false;
    }

    protected IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        animator.Play(clipHash);
    }
}
