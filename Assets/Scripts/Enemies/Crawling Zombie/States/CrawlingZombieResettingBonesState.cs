using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingZombieResettingBonesState : CrawlingZombieBaseState
{
    private float elapsedResetBonesTime = 0f;
    private float timeToResetBones = .3f;

    private bool readyToStand = false;

    public CrawlingZombieResettingBonesState(CrawlingZombieStateMachine crawlingZombieStateMachine, CrawlingZombieStateFactory crawlingZombieStateFactory) : base(crawlingZombieStateMachine, crawlingZombieStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (readyToStand)
            context.ChangeState(factory.Standing());
    }

    public override void EnterState()
    {
        AlignRotationToHips();
        AlignPositionToHips();

        context.PopulateBoneTransforms(context.RagdollBoneTransforms);

        elapsedResetBonesTime = 0f;
        readyToStand = false;

        Debug.Log("Entering resetting bones state");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting resetting bones state");
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
        return context.StandUpBoneTransforms;
    }
}
