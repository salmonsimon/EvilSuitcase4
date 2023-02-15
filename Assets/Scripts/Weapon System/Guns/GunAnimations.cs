using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimations : MonoBehaviour
{
    private Animation anim;

    private GameObject bulletContainer;

    [SerializeField] GameObject bulletShell;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        bulletContainer = GameObject.FindGameObjectWithTag(Config.PROYECTILE_CONTAINER_TAG);
    }

    public void DropBullet()
    {
        GameObject droppedBullet = Instantiate(bulletShell, bulletContainer.transform, true);
        droppedBullet.transform.position = bulletShell.transform.position;
        droppedBullet.transform.rotation = bulletShell.transform.rotation;
        droppedBullet.SetActive(true);

        Rigidbody rigidBody = droppedBullet.AddComponent<Rigidbody>();
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rigidBody.AddForce(Vector3.right * 100f);

        droppedBullet.AddComponent<BoxCollider>();
    }

    public void PlayShootAnimation(float delay)
    {
        StartCoroutine(PlayShootAnimationCoroutine(delay));
    }

    private IEnumerator PlayShootAnimationCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        anim.Play("Shoot");
    }
}
