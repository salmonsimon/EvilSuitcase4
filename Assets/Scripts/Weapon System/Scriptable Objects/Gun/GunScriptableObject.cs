using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    #region Gun Information

    [Header("Gun Information")]
    [SerializeField] private string gunName;
    public string GunName { get { return gunName; } }

    [SerializeField] private bool isHitscan = true;
    public bool IsHitscan { get { return isHitscan; } }

    #endregion

    #region Gun Configuration

    [Header("Gun Configuration")]
    [SerializeField] private ShootConfigurationScriptableObject shootConfig;
    public ShootConfigurationScriptableObject ShootConfig { get { return shootConfig; } }

    [SerializeField] private RecoilConfigurationScriptableObject recoilConfig;
    public RecoilConfigurationScriptableObject RecoilConfig { get { return recoilConfig; } }

    [DrawIf("isHitscan", true), SerializeField] private TrailConfigurationScriptableObject trailConfig;
    public TrailConfigurationScriptableObject TrailConfig { get { return trailConfig; } }

    [SerializeField] private GunDamageConfigurationScriptableObject damageConfig;
    public GunDamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } }

    [SerializeField] private AmmoConfigurationScriptableObject ammoConfig;
    public AmmoConfigurationScriptableObject AmmoConfig { get { return ammoConfig; } }

    [SerializeField] private GunAudioConfigurationScriptableObject audioConfig;
    public GunAudioConfigurationScriptableObject AudioConfig { get { return audioConfig; } }

    #endregion
}
