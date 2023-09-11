using System.Collections;
using UnityEngine;

public class RocketProjectile : Projectile
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem rocketTrailParticleSystem;
    [SerializeField] private float constantSpeed = Config.ROCKET_CONSTANT_SPEED;

    private float explosionDisableTime = Config.EXPLOSION_DISABLE_TIME;
    private float disableTime = Config.EXPLOSIVE_DISABLE_TIME;

    private AudioSource audioSource;
    private Explosion explosionObject;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.volume = GameManager.instance.GetSFXManager().GetSFXVolume();

        explosionObject = GetComponentInChildren<Explosion>(true);
    }

    private void OnEnable()
    {
        rocketTrailParticleSystem.Play();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (rigidBody)
            rigidBody.constraints = RigidbodyConstraints.None;

        if (boxCollider)
            boxCollider.enabled = true;

        model.SetActive(true);
        explosionObject.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        rigidBody.velocity = constantSpeed * (GetComponent<Rigidbody>().velocity.normalized);
    }

    protected override void Disable()
    {
        Explode();
    }

    private IEnumerator WaitToDisable(float explosionDelay, float delay)
    {
        yield return new WaitForSeconds(explosionDelay);

        explosionObject.DisableCollider();

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        Explode();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Explode();
    }

    private void Explode()
    {
        model.SetActive(false);
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        boxCollider.enabled = false;

        audioSource.Stop();

        explosionObject.gameObject.SetActive(true);

        audioSource.minDistance = 10f;
        sfx.PlayAudioClip(hitAudioClip);

        StartCoroutine(WaitToDisable(explosionDisableTime, disableTime));
    }
}