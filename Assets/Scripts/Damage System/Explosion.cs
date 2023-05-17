using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GunDamageConfigurationScriptableObject damageConfiguration;
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
        GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_EXPLOSION_AMPLITUDE, Config.CAMERASHAKE_EXPLOSION_DURATION);
    }

    private void OnDisable()
    {
        explosionCollider.enabled = false;
        explosionParticleSystem.gameObject.SetActive(false);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_EXPLOSION_AMPLITUDE, Config.CAMERASHAKE_EXPLOSION_DURATION);

        if (collision.collider.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfiguration.GetDamage(0), explosionForce, transform.position, explosionCollider.radius);
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_EXPLOSION_AMPLITUDE, Config.CAMERASHAKE_EXPLOSION_DURATION);

        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(damageConfiguration.GetDamage(0), explosionForce, transform.position, explosionCollider.radius);
        }
    }
}
