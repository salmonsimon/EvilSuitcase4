using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonShooterController))]
public class PlayerGunAnimations : MonoBehaviour
{
    [SerializeField] private Transform leftHand;

    private Gun equippedWeapon;
    private GameObject animationMagazine;

    private GameObject droppedMagazineContainer;

    private void Awake()
    {
        droppedMagazineContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG);
    }

    public void Setup(Gun equippedWeapon)
    {
        this.equippedWeapon = equippedWeapon;

        if(this.animationMagazine != null)
            Destroy(this.animationMagazine);
    }

    #region Pistol

    public void PistolDetachMagazine()
    {
        if (!animationMagazine)
        {
            animationMagazine = Instantiate(equippedWeapon.Magazine, leftHand, true);
            animationMagazine.SetActive(false);
        }
    }

    public void PistolDropMagazine()
    {
        GameObject droppedMagazine = Instantiate(equippedWeapon.Magazine, equippedWeapon.Magazine.transform.position, equippedWeapon.Magazine.transform.rotation);
        droppedMagazine.transform.localScale = equippedWeapon.Magazine.transform.lossyScale * 1.5f;
        droppedMagazine.transform.parent = droppedMagazineContainer.transform;

        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        droppedMagazine.AddComponent<BoxCollider>();

        equippedWeapon.Magazine.SetActive(false);
    }

    public void PistolRefillMagazine()
    {
        animationMagazine.SetActive(true);
    }

    public void PistolAttachMagazine()
    {
        equippedWeapon.Magazine.SetActive(true);
        animationMagazine.SetActive(false);
    }

    #endregion


    #region Machinegun

    public void MachinegunDetachMagazine() 
    { 
        if (animationMagazine)
            animationMagazine.SetActive(true);
        else
            animationMagazine = Instantiate(equippedWeapon.Magazine, leftHand, true);

        equippedWeapon.Magazine.gameObject.SetActive(false);
    }

    public void MachinegunDropMagazine() 
    {
        GameObject droppedMagazine = Instantiate(animationMagazine, animationMagazine.transform.position, animationMagazine.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        droppedMagazine.AddComponent<BoxCollider>();

        animationMagazine.SetActive(false);
    }

    public void MachinegunRefillMagazine() 
    {
        animationMagazine.SetActive(true);
    }

    public void MachinegunAttachMagazine() 
    {
        equippedWeapon.Magazine.SetActive(true);
        animationMagazine.SetActive(false);
    }

    #endregion
}
