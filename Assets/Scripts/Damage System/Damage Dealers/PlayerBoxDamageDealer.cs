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
        Vector3 playerPosition = GameManager.instance.GetPlayer().transform.position;
        Vector3 closestPosition = other.ClosestPoint(playerPosition);

        Vector3 forceDirection = (closestPosition - playerPosition).normalized;
        GameManager.instance.GetCinemachineShake().ShakeCameras(meleeWeapon.WeaponConfiguration.AttacksConfig.CameraShakeAmplitude, meleeWeapon.WeaponConfiguration.AttacksConfig.CameraShakeDuration);

        if (other.TryGetComponent(out Damageable damageable))
        {
            int damage = meleeWeapon.WeaponConfiguration.DamageConfig.GetDamage();

            if (damageable.HealthManager.LastReceivedAttackID != meleeWeapon.CurrentAttackID)
            {
                damageable.ReceiveMeleeDamage(damage, forceDirection * 35f, meleeWeapon.CurrentAttackID);

                if (damageable.TryGetComponent(out HumanoidHurtGeometry humanoidHurtGeometry))
                    GameManager.instance.GetSurfaceManager().HandleFleshImpact(damageable.transform.gameObject, closestPosition, -forceDirection, impactType, 0);
                else
                    GameManager.instance.GetSurfaceManager().HandleImpact(damageable.transform.gameObject, closestPosition, -forceDirection, impactType, 0);

                meleeWeapon.SubstractDurability();
            }
        }
        else if (other.TryGetComponent(out Rigidbody rigidBody))
        {
            rigidBody.AddForce(forceDirection * 10f, ForceMode.Impulse);

            GameManager.instance.GetSurfaceManager().HandleImpact(other.transform.gameObject, closestPosition, -forceDirection, impactType, 0);
        }
        else
        {
            GameManager.instance.GetSurfaceManager().HandleImpact(other.transform.gameObject, closestPosition, -forceDirection, impactType, 0);
        }
    }
}
