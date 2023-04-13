using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSystem : MonoBehaviour
{
    public List<MuscleComponent> MuscleComponents = new List<MuscleComponent>();

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        foreach (Rigidbody rigidBody in GetComponentsInChildren<Rigidbody>())
            MuscleComponents.Add(new MuscleComponent(rigidBody.transform));

        SetRagdoll(true, true);
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
}
