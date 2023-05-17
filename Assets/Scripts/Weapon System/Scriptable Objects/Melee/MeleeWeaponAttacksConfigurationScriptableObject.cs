using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapons/Attack Configuration", fileName = "AttackConfiguration", order = 1)]
public class MeleeWeaponAttacksConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private List<AnimationClip> attackAnimationClips;
    public List<AnimationClip> AttackAnimationClips { get { return attackAnimationClips; } }

    [SerializeField] private float attackRate = 1f;
    public float AttackRate { get { return attackRate; } }
}
