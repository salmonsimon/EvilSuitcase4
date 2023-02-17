using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private DamageConfigurationScriptableObject damageConfiguration;
    [SerializeField] private float explosionForce = 100;

    private SphereCollider explosionCollider;
    private ParticleSystem explosionParticleSystem;

    private void Awake()
    {
        explosionCollider = GetComponent<SphereCollider>();
        explosionParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        explosionCollider.enabled = true;
        explosionParticleSystem.gameObject.SetActive(true);

        explosionParticleSystem.Play();
    }

    private void OnDisable()
    {
        explosionCollider.enabled = false;
        explosionParticleSystem.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfiguration.GetDamage(0));

            collision.collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionCollider.radius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfiguration.GetDamage(0));

            other.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionCollider.radius);
        }
    }
}
