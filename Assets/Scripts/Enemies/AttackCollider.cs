using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private bool isColliding = false;
    public bool IsColliding { get { return isColliding; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Config.PLAYER_TAG))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Config.PLAYER_TAG))
        {
            isColliding = false;
        }
    }


}
