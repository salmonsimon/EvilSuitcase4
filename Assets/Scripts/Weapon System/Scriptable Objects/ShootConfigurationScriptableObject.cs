using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
public class ShootConfigurationScriptableObject : ScriptableObject
{
    #region Weapon Shoot Configuration

    [Header("Weapon Shoot Configuration")]
    [SerializeField] private LayerMask hitMask;
    public LayerMask HitMask { get { return hitMask; } }

    [SerializeField] private int pelletsPerBullet = 1;
    public int PelletsPerBullet { get { return pelletsPerBullet; } }

    [SerializeField] private Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 Spread { get { return spread; } }

    [SerializeField] private float fireRate = .25f;
    public float FireRate { get { return fireRate; } }

    #endregion

    #region Camera Configuration

    [Header("Camera Configuration")]
    [SerializeField] private float cameraShakeAmplitude = 1.0f;
    public float CameraShakeAmplitude { get { return cameraShakeAmplitude; } }

    [SerializeField] private float cameraShakeDuration = .1f;
    public float CameraShakeDuration { get { return cameraShakeDuration; } }

    #endregion
}
