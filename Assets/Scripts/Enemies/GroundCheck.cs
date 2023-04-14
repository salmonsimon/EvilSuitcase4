using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private bool isColliding = false;
    public bool IsColliding { get { return isColliding; } }

    private void OnTriggerStay(Collider other)
    {
        isColliding = true;
    }
}
