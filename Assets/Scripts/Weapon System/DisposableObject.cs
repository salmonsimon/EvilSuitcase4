using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableObject : MonoBehaviour
{
    [SerializeField] private ImpactType impactType;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null)
        {
            GameManager.instance.GetSurfaceManager().HandleImpact(collision.collider.transform.gameObject, transform.position, Vector3.up, impactType, 0);
        }
    }
}
