using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    [SerializeField] private int attackAnimations = 0;
    [SerializeField] private float maxAttackCooldown = 2f;

    private int attackAnimationToPlay = 0;
    private float timeUntilNextAttack = 0;

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Idle"))
        {
            timeUntilNextAttack -= Time.deltaTime;

            if (timeUntilNextAttack < 0)
            {
                animator.SetTrigger("Attack");
                timeUntilNextAttack = int.MaxValue;
            }
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInfo.IsName("Idle"))
        {
            attackAnimationToPlay = Random.Range(0, attackAnimations);

            animator.SetInteger("AttackIndex", attackAnimationToPlay);

            timeUntilNextAttack = Random.Range(0, maxAttackCooldown);
        }
    }
}
