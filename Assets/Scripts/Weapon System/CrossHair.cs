using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    private Ray crossHairRay;
    private RaycastHit crossHairRaycastHit;

    [SerializeField] private LayerMask hitMask;

    private void Update()
    {
        crossHairRay.origin = Camera.main.transform.position;
        crossHairRay.direction = Camera.main.transform.forward;

        if (Physics.Raycast(crossHairRay, out crossHairRaycastHit, 10000, hitMask))
            transform.position = crossHairRaycastHit.point;
        else
            transform.position = crossHairRay.origin + crossHairRay.direction.normalized * 100;
    }
}
