using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public States states;
    public float followDistance;
    public float attackDistance;
    public float escapeDistance;
    public Transform target;
    private float distanceToTarget;
    public bool playerIsTheTarget = true;
    public bool corrupted = true;
    public Coroutine calculateDistanceCO;
    public float life;

    private void Awake()
    {
        if (playerIsTheTarget)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        if (calculateDistanceCO != null)
            StopCoroutine(calculateDistanceCO);

        calculateDistanceCO = StartCoroutine(CalculateDistance());
    }

    private IEnumerator CalculateDistance()
    {
        while (corrupted)
        {
            if (target != null)
                distanceToTarget = Vector3.Distance(transform.position, target.position);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void CheckState()
    {
        switch (states)
        {
            case States.Idle:
                IdleState();
                break;
            case States.Follow:
                FollowState();
                break;
            case States.Attacking:
                AttackingState();
                break;
            case States.Dead:
                DeadState();
                break;
            default:
                break;
        }
    }

    public virtual void ChangeState(States s)
    {
        states = s;

        switch (s)
        {
            case States.Idle:
                break;
            case States.Follow:
                break;
            case States.Attacking:
                break;
            case States.Dead:
                corrupted = false;
                break;
            default:
                break;
        }
    }

    public virtual void IdleState()
    {
        if (distanceToTarget < followDistance)
            ChangeState(States.Follow);
    }

    public virtual void FollowState()
    {
        if (distanceToTarget > escapeDistance)
            ChangeState(States.Idle);
        else if (distanceToTarget < attackDistance)
            ChangeState(States.Attacking);
    }

    public virtual void AttackingState()
    {
        if (distanceToTarget > attackDistance + 0.4f)
            ChangeState(States.Follow);
    }

    public virtual void DeadState()
    {

    }

    private void LateUpdate()
    {
        CheckState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeDistance);
    }
}

public enum States { Idle, Follow, Attacking, Dead }