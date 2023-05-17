using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Melee Weapons/Durability Configuration", fileName = "DurabilityConfiguration", order = 3)]
public class MeleeWeaponDurabilityConfigurationScriptableObject : ScriptableObject
{
    [SerializeField] private Vector2 durabilityLossRange;
    public Vector2 DurabilityLossRange { get { return durabilityLossRange; } }

    public float GetDurabilityLoss()
    {
        return Random.Range(durabilityLossRange.x, durabilityLossRange.y);
    }
}
