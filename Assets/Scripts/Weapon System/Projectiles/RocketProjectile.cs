using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem rocketTrailParticleSystem;
    [SerializeField] private float constantSpeed = 25f;
    [SerializeField] private float disableTime = 2f;

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
        audioSource.Play();
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

    private IEnumerator WaitToDisable(float delay)
    {
        yield return new WaitForSeconds(delay);

        base.Disable();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        Explode();
    }

    private void Explode()
    {
        model.SetActive(false);
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        boxCollider.enabled = false;

        audioSource.Stop();

        explosionObject.gameObject.SetActive(true);

        StartCoroutine(WaitToDisable(disableTime));
    }
}
