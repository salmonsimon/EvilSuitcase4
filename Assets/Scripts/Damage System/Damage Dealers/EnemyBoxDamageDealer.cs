using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBoxDamageDealer : MonoBehaviour
{
    [SerializeField] private DamageDealerConfigScriptableObject damageDealerConfig;

    private BoxCollider boxCollider;
    private ZombieSFX zombieSFX;

    private bool initialized = false;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        zombieSFX = transform.parent.parent.GetComponent<ZombieSFX>();

        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized && zombieSFX == null)
            zombieSFX = transform.parent.parent.GetComponent<ZombieSFX>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(GetDamage(), Vector3.zero);

            boxCollider.enabled = false;

            if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
            {
                Vector3 enemyPosition = transform.root.position;
                Vector3 closestPosition = other.ClosestPoint(enemyPosition);

                Vector3 forceDirection = (closestPosition - enemyPosition).normalized;

                GameManager.instance.GetBloodManager().SpawnBloodOnHit(damageable.transform, closestPosition, -forceDirection);
                zombieSFX.PlayRandomHitImpactAudioClip();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Damageable damageable))
        {
            damageable.ReceiveDamage(GetDamage(), Vector3.zero);

            boxCollider.enabled = false;

            if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
            {
                Vector3 enemyPosition = transform.root.position;
                Vector3 closestPosition = collision.collider.ClosestPoint(enemyPosition);

                Vector3 forceDirection = (closestPosition - enemyPosition).normalized;

                GameManager.instance.GetBloodManager().SpawnBloodOnHit(damageable.transform, closestPosition, -forceDirection);
                zombieSFX.PlayRandomHitImpactAudioClip();
            }
        }
    }

    private int GetDamage()
    {
        return Random.Range(damageDealerConfig.MinMaxDamage.x, damageDealerConfig.MinMaxDamage.y);
    }
}
