using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Serpens : Enemy, IDamageable
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public AnimationCurve turnCurveVelocity;
    public float turnVelocity;

    private Transform actualPoint;
    private bool isAttacking;

    public override void FollowState()
    {
        base.FollowState();
        if (states != States.Follow) return;
        agent.SetDestination(target.position);
    }

    public override void IdleState()
    {
        base.IdleState();
        if (states != States.Idle) return;
        if (agent.remainingDistance <= 0.5f)
            SelectNextWayPoint();
    }

    private void SelectNextWayPoint()
    {
        actualPoint = actualPoint == null ? wayPoints[0] : actualPoint;

        int actualIndex = wayPoints.IndexOf(actualPoint);
        int nextPointIndex = actualIndex == wayPoints.Count - 1 ? 0 : actualIndex + 1;

        actualPoint = wayPoints[nextPointIndex];
        agent.isStopped = false;
        agent.SetDestination(actualPoint.position);
    }

    private void ComeBackToTheRoute()
    {
        actualPoint = wayPoints.Aggregate(wayPoints[0], (closer, next) => (transform.position - next.position).magnitude < (transform.position - closer.position).magnitude ? next : closer);

        agent.isStopped = false;
        agent.SetDestination(actualPoint.position);
    }

    public override void AttackingState()
    {
        base.AttackingState();
        if (states != States.Attacking) return;
        if (isAttacking) return;

        isAttacking = true;
        StartCoroutine(Attack());
    }

    private IEnumerator TurnToTarget()
    {
        float currentCurveValue = 0;

        while (true)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            currentCurveValue = Mathf.MoveTowards(currentCurveValue, 1, turnVelocity * Time.fixedDeltaTime);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnCurveVelocity.Evaluate(currentCurveValue));
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Attack()
    {
        Coroutine turnToTargetCo = StartCoroutine(TurnToTarget());



        yield return null;
    }

    public override void DeadState()
    {
        base.DeadState();
        if (states != States.Dead) return;
    }

    public override void ChangeState(States s)
    {
        base.ChangeState(s);
        switch (s)
        {
            case States.Idle:
                ComeBackToTheRoute();
                break;
            case States.Follow:
                break;
            case States.Attacking:
                break;
            case States.Dead:
                ComeBackToTheRoute();
                break;
            default:
                break;
        }
    }

    public void ReceiveDamage(float value)
    {
        if (!corrupted) return;
        life -= value;
        if (life <= 0)
        {
            ChangeState(States.Dead);
        }
    }
}
