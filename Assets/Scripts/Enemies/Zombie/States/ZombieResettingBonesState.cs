using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieResettingBonesState : ZombieBaseState
{
    private float elapsedResetBonesTime = 0f;
    private float timeToResetBones = .3f;

    private bool readyToStand = false;
    private bool isFacingDown = false;

    public ZombieResettingBonesState(ZombieStateMachine zombieStateMachine, ZombieStateFactory zombieStateFactory) : base(zombieStateMachine, zombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (readyToStand)
            context.ChangeState(factory.Standing());
    }

    public override void EnterState()
    {
        isFacingDown = context.FellCheck.transform.forward.y < 0;

        AlignRotationToHips();
        AlignPositionToHips();

        context.PopulateBoneTransforms(context.RagdollBoneTransforms);

        elapsedResetBonesTime = 0f;
        readyToStand = false;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

        BoneTransform[] standupBones = GetStandUpBoneTransforms();

        for (int boneIndex = 0; boneIndex < context.Bones.Length; boneIndex++)
        {
            context.Bones[boneIndex].localPosition = Vector3.Lerp(
                context.RagdollBoneTransforms[boneIndex].Position,
                standupBones[boneIndex].Position,
                elapsedPercentage);

            context.Bones[boneIndex].localRotation = Quaternion.Lerp(
                context.RagdollBoneTransforms[boneIndex].Rotation,
                standupBones[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >= 1)
        {
            readyToStand = true;
        }

        CheckSwitchStates();
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = context.HipsBone.position;
        Quaternion originalHipsRotation = context.HipsBone.rotation;

        Vector3 desiredDirection = context.FellCheck.transform.up;

        if (!isFacingDown)
            desiredDirection *= -1;

        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(context.transform.forward, desiredDirection);
        context.transform.rotation *= fromToRotation;

        context.HipsBone.position = originalHipsPosition;
        context.HipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = context.HipsBone.position;
        context.transform.position = context.HipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0;
        positionOffset = context.transform.rotation * positionOffset;
        context.transform.position -= positionOffset;

        if (Physics.Raycast(context.transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            context.transform.position = new Vector3(context.transform.position.x, hitInfo.point.y, context.transform.position.z);
        }

        context.HipsBone.position = originalHipsPosition;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        if (isFacingDown)
            return context.StandUpFromBellyBoneTransforms;
        else
            return context.StandUpFromBackBoneTransforms;
    }
}