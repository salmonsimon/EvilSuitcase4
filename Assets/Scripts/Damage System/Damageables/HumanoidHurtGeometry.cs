using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class HumanoidHurtGeometry : Damageable
{
    [SerializeField] private HumanoidBodyPart bodyPart;
    public HumanoidBodyPart BodyPart { get { return bodyPart; } }

    [SerializeField] private bool hasRagdoll = true;

    private RagdollSystem ragdollSystem;
    public RagdollSystem RagdollSystem { get { return ragdollSystem; } }

    private MuscleComponent muscleComponent;

    protected override void Awake()
    {
        healthManager = GetComponentInParent<HealthManager>();
    }

    private void Start()
    {
        if (hasRagdoll)
        {
            ragdollSystem = GetComponentInParent<RagdollSystem>();
            muscleComponent = GetComponent<MuscleComponent>();
        }
    }

    public override void ReceiveMeleeDamage(int damage, Vector3 force, int attackID)
    {
        if (HealthManager.LastReceivedAttackID != attackID)
        {
            HealthManager.LastReceivedAttackID = attackID;

            ReceiveDamage(damage, force);
        }
    }

    public override void ReceiveDamage(int damage, Vector3 force)
    {
        float damageMultiplier = 1f;

        switch (bodyPart)
        {
            case HumanoidBodyPart.Head:
                damageMultiplier *= 2f;
                break;

            case HumanoidBodyPart.Torso:
                damageMultiplier *= 1f;
                break;

            case HumanoidBodyPart.Arm:
                damageMultiplier *= .7f;
                break;

            case HumanoidBodyPart.Hand:
                damageMultiplier *= .5f;
                break;

            case HumanoidBodyPart.Leg:
                damageMultiplier *= .7f;
                break;

            case HumanoidBodyPart.Foot:
                damageMultiplier *= .5f;
                break;
        }

        if (HealthManager.IsAlive)
            healthManager.ReceiveDamage((int)(damage * damageMultiplier));

        if (hasRagdoll)
            ragdollSystem.ApplyForce(muscleComponent, force);
    }

    public override void ReceiveDamage(int damage, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (HealthManager.IsAlive)
            healthManager.ReceiveDamage(damage);

        Vector3 force = transform.position - explosionPosition;

        if (hasRagdoll)
            ragdollSystem.ApplyForce(muscleComponent, force * 5f);
    }
}
