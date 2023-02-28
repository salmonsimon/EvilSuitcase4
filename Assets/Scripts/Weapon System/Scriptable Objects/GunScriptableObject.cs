using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

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

    [HideInInspector, SerializeField] private TrailConfigurationScriptableObject trailConfig;
    public TrailConfigurationScriptableObject TrailConfig { get { return trailConfig; } }

    [SerializeField] private DamageConfigurationScriptableObject damageConfig;
    public DamageConfigurationScriptableObject DamageConfig { get { return damageConfig; } }

    [SerializeField] private AmmoConfigurationScriptableObject ammoConfig;
    public AmmoConfigurationScriptableObject AmmoConfig { get { return ammoConfig; } }

    [SerializeField] private AudioConfigurationScriptableObject audioConfig;
    public AudioConfigurationScriptableObject AudioConfig { get { return audioConfig; } }

    #endregion

    #region Custom Editor

#if UNITY_EDITOR
    [CustomEditor(typeof(GunScriptableObject))]
    public class RandomScript_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GunScriptableObject script = (GunScriptableObject)target;

            if (script.isHitscan)
            {
                var gameObject = EditorGUILayout.ObjectField("Trail Config", script.TrailConfig, typeof(TrailConfigurationScriptableObject), true) as TrailConfigurationScriptableObject;
            }
        }
    }
#endif

    #endregion
}
