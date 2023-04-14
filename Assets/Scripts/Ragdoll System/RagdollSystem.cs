using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RagdollSystem : MonoBehaviour
{
    public List<MuscleComponent> MuscleComponents = new List<MuscleComponent>();
    public List<MuscleComponent> GroundedMuscleComponents = new List<MuscleComponent>();

    private Animator animator;

    private bool onHitRecovery = false;
    private float timeToRecover = 1f;
    private float elapsedRecoverTime = 0f;
    private float hitRecoverMultiplier = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        foreach (Rigidbody rigidBody in GetComponentsInChildren<Rigidbody>())
        {
            MuscleComponent newMuscleComponent = rigidBody.AddComponent<MuscleComponent>();
            newMuscleComponent.Transform = rigidBody.transform;
            newMuscleComponent.Rigidbody = rigidBody;
            newMuscleComponent.Collider = rigidBody.transform.GetComponent<Collider>();
            newMuscleComponent.BodyPart = rigidBody.transform.GetComponent<HumanoidHurtGeometry>().BodyPart;

            MuscleComponents.Add(newMuscleComponent);

            if (newMuscleComponent.BodyPart == HumanoidBodyPart.Foot)
            {
                ConfigurableJoint configurableJoint = newMuscleComponent.AddComponent<ConfigurableJoint>();
                configurableJoint.configuredInWorldSpace = true;
                newMuscleComponent.ConfigurableJoint = configurableJoint;

                GroundedMuscleComponents.Add(newMuscleComponent);
            }
        }

        SetRagdoll(true, true);
    }

    private void LateUpdate()
    {
        if (onHitRecovery)
        {
            elapsedRecoverTime += Time.deltaTime * hitRecoverMultiplier;
            float elapsedRecoverTimePercentage = elapsedRecoverTime / timeToRecover;

            foreach (MuscleComponent muscleComponent in MuscleComponents)
            {
                muscleComponent.transform.localPosition = Vector3.Lerp(muscleComponent.StoredLocalPosition, muscleComponent.transform.localPosition, elapsedRecoverTimePercentage);
                muscleComponent.transform.localRotation = Quaternion.Lerp(muscleComponent.StoredLocalRotation, muscleComponent.transform.localRotation, elapsedRecoverTimePercentage);
            }

            if (elapsedRecoverTime >= timeToRecover)
                onHitRecovery = false;
        }
    }

    public void ApplyForce(MuscleComponent muscleComponent, Vector3 force)
    {
        StopAllCoroutines();
        ResetRagdoll();

        StartCoroutine(ApplyForceCoroutine(muscleComponent, force));
    }

    public IEnumerator ApplyForceCoroutine(MuscleComponent muscleComponent, Vector3 force)
    {
        SetRagdoll(false, false);

        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
        {
            if (groundedMuscleComponent != muscleComponent)
            {
                groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
                groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
                groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;

                if (groundedMuscleComponent.transform.childCount > 0)
                    groundedMuscleComponent.ConfigurableJoint.anchor = groundedMuscleComponent.transform.GetChild(0).localPosition;
            }
        }

        muscleComponent.Rigidbody.AddForce(force, ForceMode.Impulse);

        yield return null;

        if (muscleComponent.BodyPart != HumanoidBodyPart.Foot)
        {
            foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
            {
                groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Free;
                groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Free;
                groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Free;
            }
        }

        yield return null;

        SetRagdoll(false, true);

        yield return null;

        foreach (MuscleComponent muscle in MuscleComponents)
            muscle.StoreLocalPositionAndRotation();

        SetRagdoll(true, true);

        elapsedRecoverTime = 0;
        onHitRecovery = true;
    }

    public void SetRagdoll(bool isAnimated, bool gravity)
    {
        animator.enabled = isAnimated;

        foreach (MuscleComponent muscleComponent in MuscleComponents)
        {
            muscleComponent.Rigidbody.useGravity = gravity;

            if (muscleComponent.Transform == transform)
            {
                muscleComponent.Collider.isTrigger = !isAnimated;
                muscleComponent.Rigidbody.isKinematic = !isAnimated;

                continue;
            }

            muscleComponent.Collider.isTrigger = isAnimated;
            muscleComponent.Rigidbody.isKinematic = isAnimated;
        }

        if (!isAnimated)
            GetComponent<ZombieSFX>().PlayRandomHurtAudioClip();
    }

    private void ResetRagdoll()
    {
        SetRagdoll(true, true);

        onHitRecovery = false;

        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
        {
            groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Free;
            groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Free;
            groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Free;
        }
    }
}
