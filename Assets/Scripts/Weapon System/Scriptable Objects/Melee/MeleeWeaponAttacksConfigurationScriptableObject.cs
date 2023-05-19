using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapons/Attack Configuration", fileName = "AttackConfiguration", order = 1)]
public class MeleeWeaponAttacksConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private List<AnimationClip> attackAnimationClips;
    public List<AnimationClip> AttackAnimationClips { get { return attackAnimationClips; } }

    [SerializeField] private float attackRate = 1f;
    public float AttackRate { get { return attackRate; } }

    #region Camera Configuration

    [Header("Camera Configuration")]
    [SerializeField] private float cameraShakeAmplitude = 6.0f;
    public float CameraShakeAmplitude { get { return cameraShakeAmplitude; } }

    [SerializeField] private float cameraShakeDuration = .3f;
    public float CameraShakeDuration { get { return cameraShakeDuration; } }

    #endregion
}
