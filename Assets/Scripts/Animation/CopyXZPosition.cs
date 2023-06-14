using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyXZPosition : MonoBehaviour
{
    [SerializeField] private Transform lookat;

    private void LateUpdate()
    {
        transform.position = new Vector3(lookat.position.x, transform.position.y, lookat.position.z);
    }
}
