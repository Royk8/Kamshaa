using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Cucarron : Enemy, IEnemy
{
    public NavMeshAgent agent;
    public Animator anim;
    public Rigidbody rb;
    public AnimationClip movementClip;
    public bool readyToMove = true;
    public float chargeDuration;
    public float chargeForce;

    private bool isAttacking = false;

    private Vector3 initalPosition;

    private void Start()
    {
        initalPosition = transform.position;
        isAttacking = false;
        rb.isKinematic = true;
        agent.enabled = true;
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
        //isAttacking = true;
        //agent.enabled = false;
        //rb.isKinematic = false;
        //transform.LookAt(target, Vector3.up);
        //rb.AddForce(transform.forward * chargeForce);
        //yield return new WaitForSeconds(chargeDuration);
        //rb.isKinematic = true;
        //agent.enabled = true;
        //isAttacking = false;

        isAttacking = true;
        float normalVelocity = agent.speed;
        agent.speed = chargeForce;
        transform.LookAt(target, Vector3.up);
        agent.SetDestination(target.position + (transform.forward * 1.4f));
        yield return new WaitForSeconds(chargeDuration);
        agent.speed = normalVelocity;
        isAttacking = false;
    }

    public override void FollowState()
    {
        base.FollowState();
        anim.SetBool("IsMoving", true);
        if (readyToMove)
        {
            agent.SetDestination(target.position);
        }
    }

    public override void IdleState()
    {
        base.IdleState();
        if (transform.position == initalPosition)
        {
            readyToMove = false;
            anim.SetBool("IsMoving", false);
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

    public void GetStuned()
    {

    }

    public void ReceiveDamage()
    {

    }
}
