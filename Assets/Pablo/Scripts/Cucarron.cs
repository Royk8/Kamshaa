using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class Cucarron : Enemy, IEnemy
{
    public NavMeshAgent agent;
    public Animator anim;
    public AnimationClip movementClip;
    public bool readyToMove = true;
    public float chargeDuration;
    public float chargeForce;
    public AnimationCurve turnCurveVelocity;
    public float turnVelocity;

    private bool isAttacking = false;
    private bool buried = true;
    private bool alreadyTurned = false;

    private Vector3 initalPosition;

    private void Start()
    {
        initalPosition = transform.position;
        isAttacking = false;
        buried = true;
    }

    public override void ChangeState(States s)
    {
        base.ChangeState(s);
        switch (s)
        {
            case States.Idle:
                readyToMove = false;
                agent.SetDestination(initalPosition);
                break;
            case States.Follow:
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == movementClip.name)
                    readyToMove = true;
                break;
            case States.Attacking:
                break;
            case States.Dead:
                break;
            default:
                break;
        }
    }

    public override void AttackingState()
    {
        base.AttackingState();
        if (!isAttacking)
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        float normalVelocity = agent.speed;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation));
        yield return new WaitUntil(() => alreadyTurned);
        agent.speed = chargeForce;
        agent.SetDestination(target.position + (transform.forward * 1.4f));
        yield return new WaitForSeconds(chargeDuration);
        agent.speed = normalVelocity;
        isAttacking = false;
    }

    private IEnumerator TurnToTarget(Quaternion targetRotation)
    {
        Quaternion initialRotation = transform.rotation;
        float currentCurveValue = 0;

        while (currentCurveValue != 1)
        {
            currentCurveValue = Mathf.MoveTowards(currentCurveValue, 1, turnVelocity * Time.fixedDeltaTime);

            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, turnCurveVelocity.Evaluate(currentCurveValue));
            yield return new WaitForFixedUpdate();
        }
        alreadyTurned = true;
    }

    public override void FollowState()
    {
        base.FollowState();
        anim.SetBool("IsMoving", true);
        buried = false;
        if (readyToMove)
        {
            agent.SetDestination(target.position);
        }
    }

    public override void IdleState()
    {
        base.IdleState();
        if (buried) return;
        if ((transform.position - initalPosition).magnitude < 0.05f)
        {
            readyToMove = false;
            anim.SetBool("IsMoving", false);
            buried = true;
        }
    }

    public override void DeadState()
    {
        base.DeadState();
        agent.isStopped = true;
    }

    //this is called from an animation event in the animation Exit_Idle
    public void IsReadyToMove()
    {
        readyToMove = true;
    }

    public IEnumerator GetStuned(float timeStuned)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(timeStuned);
        agent.isStopped = false;
    }

    public void ReceiveDamage(int damageDealed)
    {
        life -= damageDealed;
        if (life <= 0)
        {
            ChangeState(States.Dead);
        }
    }
}
