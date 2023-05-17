using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    [SerializeField] private GunDamageConfigurationScriptableObject damageConfig;
    public GunDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        transform.SetParent(collision.collider.transform);
        Disable();

        if (collision.collider.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), Vector3.zero);
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        base.OnTriggerEnter(collider);

        transform.SetParent(collider.transform, true);
        Disable();

        if (collider.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled), Vector3.zero);
    }
}
