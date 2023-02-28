using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonShooterController))]
public class PlayerGunAnimations : MonoBehaviour
{
    [SerializeField] private Transform leftHand;

    private Gun equippedWeapon;
    private GameObject animationObject;

    private GameObject droppedObjectContainer;

    private void Awake()
    {
        droppedObjectContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG);
    }

    public void Setup(Gun equippedWeapon)
    {
        this.equippedWeapon = equippedWeapon;

        if(this.animationObject != null)
            Destroy(this.animationObject);
    }

    #region Pistol

    public void PistolDetachMagazine()
    {
        if (!animationObject)
        {
            animationObject = Instantiate(equippedWeapon.AnimationObject, leftHand, true);
            animationObject.SetActive(false);
        }
    }

    public void PistolDropMagazine()
    {
        GameObject droppedMagazine = Instantiate(equippedWeapon.AnimationObject, equippedWeapon.AnimationObject.transform.position, equippedWeapon.AnimationObject.transform.rotation);
        droppedMagazine.transform.parent = droppedObjectContainer.transform;

        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        droppedMagazine.AddComponent<BoxCollider>();

        equippedWeapon.AnimationObject.SetActive(false);
    }

    public void PistolRefillMagazine()
    {
        animationObject.SetActive(true);
    }

    public void PistolAttachMagazine()
    {
        equippedWeapon.AnimationObject.SetActive(true);
        animationObject.SetActive(false);
    }

    #endregion

    #region Machinegun

    public void MachinegunDetachMagazine() 
    { 
        if (animationObject)
            animationObject.SetActive(true);
        else
            animationObject = Instantiate(equippedWeapon.AnimationObject, leftHand, true);

        equippedWeapon.AnimationObject.gameObject.SetActive(false);
    }

    public void MachinegunDropMagazine() 
    {
        GameObject droppedMagazine = Instantiate(animationObject, animationObject.transform.position, animationObject.transform.rotation);
        droppedMagazine.transform.parent = droppedObjectContainer.transform;

        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        droppedMagazine.AddComponent<BoxCollider>();

        animationObject.SetActive(false);
    }

    public void MachinegunRefillMagazine() 
    {
        animationObject.SetActive(true);
    }

    public void MachinegunAttachMagazine() 
    {
        equippedWeapon.AnimationObject.SetActive(true);
        animationObject.SetActive(false);
    }

    #endregion

    #region Uzi

    public void UziDetachMagazine()
    {
        if (animationObject)
            animationObject.SetActive(true);
        else
            animationObject = Instantiate(equippedWeapon.AnimationObject, leftHand, true);

        equippedWeapon.AnimationObject.gameObject.SetActive(false);
    }

    public void UziDropMagazine()
    {
        GameObject droppedMagazine = Instantiate(animationObject, animationObject.transform.position, animationObject.transform.rotation);
        droppedMagazine.transform.parent = droppedObjectContainer.transform;

        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        droppedMagazine.AddComponent<BoxCollider>();

        animationObject.SetActive(false);
    }

    public void UziRefillMagazine()
    {
        animationObject.SetActive(true);
    }

    public void UziAttachMagazine()
    {
        equippedWeapon.AnimationObject.SetActive(true);
        animationObject.SetActive(false);
    }

    #endregion

    #region Crossbow

    public void CrossbowReloadAnimation()
    {
        equippedWeapon.GetComponentInChildren<GunAnimations>().PlayReloadAnimation(0f);
    }

    #endregion
}
