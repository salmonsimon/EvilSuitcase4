using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalLookSensitivity = 2f;
    [SerializeField] private float aimSensitivity = 1f;

    [SerializeField] private Transform lookAt;

    [Header("Rigging")]
    [SerializeField] private List<Rig> idleRigs;
    [SerializeField] private List<Rig> aimRigs;

    [Header("Equiped Weapon")]
    [SerializeField] private GameObject equipedWeapon;

    #region Obejct References

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private Animator animator;

    #endregion

    #region Logic Variables

    private Vector3 aimDirection;
    private bool aiming = false;

    #endregion

    #region Parameters

    private float movementSpeed = 5f;
    private float aimMovementSpeed = 3f;

    #endregion


    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (starterAssetsInputs.aim)
        {
            aiming = true;

            aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetAbleToSprint(false);
            thirdPersonController.MoveSpeed = aimMovementSpeed;
        }
        else 
        {
            aiming = false;

            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalLookSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetAbleToSprint(true);
            thirdPersonController.MoveSpeed = movementSpeed;
        }

        if (starterAssetsInputs.shoot && equipedWeapon != null)
        {
            //equipedWeapon.Attack();
        }
    }

    private void FixedUpdate()
    {
        if (aiming)
        {
            if (Vector3.Dot(transform.forward, aimDirection) > .9f)
            {
                foreach (Rig rig in aimRigs)
                    rig.weight = Mathf.Lerp(rig.weight, 1f, Time.deltaTime * 20f);
            }
        }
        else
        {
            foreach (Rig rig in aimRigs)
                rig.weight = Mathf.Lerp(rig.weight, 0f, Time.deltaTime * 20f);
        }
    }

    private void LateUpdate()
    {
        if (aiming)
        {
            Vector3 worldAimTarget = lookAt.position;
            worldAimTarget.y = transform.position.y;

            aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
    }
}
