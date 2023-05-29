using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon", menuName = "Melee Weapons/Melee Weapon", order = 0)]
public class MeleeWeaponScriptableObject : ScriptableObject
{
    #region Weapon Information

    [Header("Weapon Information")]
    [SerializeField] private string weaponName;
    public string WeaponName { get { return weaponName; } }

    [SerializeField] private Sprite weaponSprite;
    public Sprite WeaponSprite { get { return weaponSprite; } }

    #endregion

    #region Weapon Configuration

    [Header("Weapon Configuration")]
    [SerializeField] private MeleeWeaponAttacksConfigurationScriptableObject attacksConfig;
    public MeleeWeaponAttacksConfigurationScriptableObject AttacksConfig { get { return attacksConfig; } }

    [SerializeField] private MeleeWeaponDamageConfigurationScriptableObject damageConfig;
    public MeleeWeaponDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } }

    [SerializeField] private MeleeWeaponDurabilityConfigurationScriptableObject durabilityConfig;
    public MeleeWeaponDurabilityConfigurationScriptableObject DurabilityConfig { get { return durabilityConfig; } }

    [SerializeField] private MeleeWeaponAudioConfigurationScriptableObject audioConfig;
    public MeleeWeaponAudioConfigurationScriptableObject AudioConfig { get { return audioConfig; } }

    [SerializeField] private CrosshairConfigurationScriptableObject crossHairConfig;
    public CrosshairConfigurationScriptableObject CrossHairConfig { get { return crossHairConfig; } }

    #endregion
}
