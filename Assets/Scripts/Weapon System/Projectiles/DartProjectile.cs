using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartProjectile : Projectile
{
    [SerializeField] private DamageConfigurationScriptableObject damageConfig;
    public DamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } set { damageConfig = value; } }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        transform.SetParent(collision.collider.transform);

        if (collision.collider.TryGetComponent(out Damageable damageable))
            damageable.ReceiveDamage(damageConfig.GetDamage(distanceTraveled));

        Disable();
    }
}
