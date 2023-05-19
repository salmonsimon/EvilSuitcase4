using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Guns/Recoil Configuration", fileName = "RecoilConfiguration", order = 8)]
public class RecoilConfigurationScriptableObject : ScriptableObject
{
    #region Configuration

    [Header("Recoil Pattern")]
    [SerializeField] private Vector2 recoilPattern;
    public Vector2 RecoilPattern { get { return recoilPattern; } }

    [Header("Recoil Timing")]
    [SerializeField] private float recoilSnappiness = 1f;
    public float RecoilSnappiness { get { return recoilSnappiness; } }

    [SerializeField] private float recoilRecoverySpeed = 1f;
    public float RecoilRecoverySpeed { get { return recoilRecoverySpeed;} }

    [SerializeField] private float maxRecoilTime = 2f;
    public float MaxRecoilTime { get { return maxRecoilTime; } }

    #endregion
}
