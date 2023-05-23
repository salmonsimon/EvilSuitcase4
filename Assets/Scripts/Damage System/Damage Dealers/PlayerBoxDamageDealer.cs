using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxDamageDealer : MonoBehaviour
{
    #region Object References

    [SerializeField] private MeleeWeapon meleeWeapon;

    private Collider meleeAttackCollider;

    #endregion

    #region Parameters

    [SerializeField] private ImpactType impactType;

    #endregion

    private void Start()
    {
        meleeAttackCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 forceDirection = other.transform.position - GameManager.instance.GetPlayer().transform.position;
        GameManager.instance.GetCinemachineShake().ShakeCamera(meleeWeapon.WeaponConfiguration.AttacksConfig.CameraShakeAmplitude, meleeWeapon.WeaponConfiguration.AttacksConfig.CameraShakeDuration);

        if (other.TryGetComponent(out Damageable damageable))
        {
            int damage = meleeWeapon.WeaponConfiguration.DamageConfig.GetDamage();

            damageable.ReceiveMeleeDamage(damage, forceDirection * 150f, meleeWeapon.CurrentAttackID);

            if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
                GameManager.instance.GetSurfaceManager().HandleFleshImpact(damageable.transform.gameObject, Vector3.zero, -forceDirection, impactType, 0);
            else
                GameManager.instance.GetSurfaceManager().HandleImpact(damageable.transform.gameObject, Vector3.zero, -forceDirection, impactType, 0);

            meleeWeapon.SubstractDurability();
        }
        else
        {
            GameManager.instance.GetSurfaceManager().HandleImpact(other.transform.gameObject, Vector3.zero, -forceDirection, impactType, 0);
        }
    }
}
