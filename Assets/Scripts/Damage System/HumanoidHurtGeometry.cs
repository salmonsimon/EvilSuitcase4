using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class HumanoidHurtGeometry : Damageable
{
    [SerializeField] private HumanoidBodyPart bodyPart;

    public override void ReceiveDamage(int damage)
    {
        float damageMultiplier = 1f;

        switch (bodyPart)
        {
            case HumanoidBodyPart.Head:
                damageMultiplier *= 2f;
                break;

            case HumanoidBodyPart.Torso:
                damageMultiplier *= 1f;
                break;

            case HumanoidBodyPart.Arm:
                damageMultiplier *= .7f;
                break;

            case HumanoidBodyPart.Hand:
                damageMultiplier *= .5f;
                break;

            case HumanoidBodyPart.Leg:
                damageMultiplier *= .7f;
                break;

            case HumanoidBodyPart.Foot:
                damageMultiplier *= .5f;
                break;
        }

        Debug.Log("Damage Received: " + (int)(damage * damageMultiplier));
        healthManager.ReceiveDamage((int)(damage * damageMultiplier));
    }
}
