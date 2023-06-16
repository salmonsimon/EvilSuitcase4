using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Utils;

public class RagdollSystem : MonoBehaviour
{
    [SerializeField] private Transform skeleton;

    [SerializeField] private Collider mapCollider;
    public Collider MapCollider { get { return mapCollider; } }

    public List<MuscleComponent> MuscleComponents = new List<MuscleComponent>();
    public List<MuscleComponent> GroundedMuscleComponents = new List<MuscleComponent>();

    private Animator animator;
    private ZombieStateMachine stateMachine;

    [SerializeField] private bool onHitRecovery = false;
    private float timeToRecover = 1f;
    [SerializeField] private float elapsedRecoverTime = 0f;
    private float hitRecoverMultiplier = 2f;

    [SerializeField] private float MaxHitForce = 500f;
    [SerializeField] private float accumulatedHitForce = 0;
    private float accumulatedHitForceRecoveryMultiplier = 5f;

    [SerializeField] private bool ragdollMode = false;
    public bool RagdollMode { get { return ragdollMode; } set { ragdollMode = value; } }

    public delegate void OnRagdollActivateDelegate();
    public event OnRagdollActivateDelegate OnRagdollActivate;

    private void OnEnable()
    {
        stateMachine.HealthManager.OnDeath += OnDeath;
        stateMachine.HealthManager.OnRevival += OnRevival;
    }

    private void OnDisable()
    {
        stateMachine.HealthManager.OnDeath -= OnDeath;
        stateMachine.HealthManager.OnRevival -= OnRevival;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<ZombieStateMachine>();

        foreach (Rigidbody rigidBody in skeleton.GetComponentsInChildren<Rigidbody>())
        {
            MuscleComponent newMuscleComponent = rigidBody.AddComponent<MuscleComponent>();
            newMuscleComponent.Transform = rigidBody.transform;
            newMuscleComponent.Rigidbody = rigidBody;
            newMuscleComponent.Collider = rigidBody.transform.GetComponent<Collider>();
            newMuscleComponent.BodyPart = rigidBody.transform.GetComponent<HumanoidHurtGeometry>().BodyPart;

            newMuscleComponent.Collider.enabled = false;
            newMuscleComponent.Collider.enabled = true;

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
                ResetRagdoll();
        }

        if (accumulatedHitForce > 0)
            accumulatedHitForce -= Time.deltaTime * accumulatedHitForceRecoveryMultiplier;
    }

    public void ApplyForce(MuscleComponent muscleComponent, Vector3 force)
    {
        if (!stateMachine.CurrentState.ToSafeString().Equals("ZombieRagdollState") &&
            !stateMachine.CurrentState.ToSafeString().Equals("ZombieDeadState"))
        {
            accumulatedHitForce += force.magnitude;

            if (accumulatedHitForce > MaxHitForce)
            {
                accumulatedHitForce = 0;
                OnRagdollActivate();

                StopAllCoroutines();

                muscleComponent.Rigidbody.AddForce(force, ForceMode.Impulse);

                return;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(ApplyForceCoroutine(muscleComponent, force));
            }
        }
        else
            muscleComponent.Rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public IEnumerator ApplyForceCoroutine(MuscleComponent muscleComponent, Vector3 force)
    {
        yield return null;

        if (!stateMachine.HealthManager.IsAlive)
            yield break;

        mapCollider.isTrigger = true;

        yield return null;

        SetRagdoll(false, false);

        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
        {
            if (!RagdollMode && groundedMuscleComponent != muscleComponent)
            {
                groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Locked;
                groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Locked;
                groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Locked;

                if (groundedMuscleComponent.transform.childCount > 0)
                    groundedMuscleComponent.ConfigurableJoint.anchor = groundedMuscleComponent.transform.GetChild(0).localPosition;
            }
            else
            {
                groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Free;
                groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Free;
                groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Free;
            }
        }

        if (!RagdollMode)
            muscleComponent.Rigidbody.AddForce(force, ForceMode.Impulse);

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

    public void ResetRagdoll()
    {
        onHitRecovery = false;

        if (stateMachine.HealthManager.IsAlive)
            mapCollider.isTrigger = false;

        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
        {
            groundedMuscleComponent.ConfigurableJoint.xMotion = ConfigurableJointMotion.Free;
            groundedMuscleComponent.ConfigurableJoint.yMotion = ConfigurableJointMotion.Free;
            groundedMuscleComponent.ConfigurableJoint.zMotion = ConfigurableJointMotion.Free;
        }
    }

    private void OnDeath()
    {
        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
            if (groundedMuscleComponent.TryGetComponent(out Collider collider))
                collider.enabled = false;
    }

    private void OnRevival()
    {
        foreach (MuscleComponent groundedMuscleComponent in GroundedMuscleComponents)
            if (groundedMuscleComponent.TryGetComponent(out Collider collider))
                collider.enabled = true;
    }
}
