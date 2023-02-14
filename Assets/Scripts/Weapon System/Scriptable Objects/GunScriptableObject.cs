using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    public string GunName;

    public ShootConfigurationScriptableObject ShootConfig;
    public TrailConfigurationScriptableObject TrailConfig;
    public DamageConfigurationScriptableObject DamageConfig;
    public AmmoConfigurationScriptableObject AmmoConfig;
    public AudioConfigurationScriptableObject AudioConfig;
}
