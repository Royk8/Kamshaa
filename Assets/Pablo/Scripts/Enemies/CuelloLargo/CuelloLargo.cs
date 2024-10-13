using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CuelloLargo : Enemy, IDamageable, IStuneable
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public AnimationCurve turnCurveVelocity;
    public float turnVelocity;
    public float attackingRate;
    public float attackAnimationDelay;
    public float damage;
    public float unCorruptedSpeed;
    public Animator anim;
    public Metamorfosis metamorfosis;
    public GameObject hitCapsuleRef;
    public Transform startHitCapsulePos, endHitCapsulePos;
    public LayerMask hitMask;

    private Transform actualPoint;
    private bool isAttacking;
    private bool alreadyTurned = false;

    public override void AttackingState()
    {
        base.AttackingState();
        if (states != States.Attacking) return;
        agent.isStopped = true;
        if (isAttacking) return;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation));
        hitCapsuleRef.SetActive(false);

        yield return new WaitUntil(() => alreadyTurned);

        anim.SetTrigger("atacar");

        yield return new WaitForSeconds(attackAnimationDelay);

        hitCapsuleRef.SetActive(true);
        Invoke(nameof(DeactivateHitCapsuleRef), 0.5f);
        Collider[] collidersAffected = Physics.OverlapCapsule(startHitCapsulePos.position, endHitCapsulePos.position, hitCapsuleRef.transform.localScale.x / 2, hitMask);
        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i] == GetComponent<Collider>()) continue;

            Debug.Log($"{collidersAffected[i].name} afectado por {transform.name}");
            if (collidersAffected[i].TryGetComponent(out IDamageable damageable))
            {
                Debug.Log($"{collidersAffected[i].name} dañado por {transform.name}");
                damageable.ReceiveDamage(damage);
            }
        }

        yield return new WaitForSeconds(attackingRate);
        isAttacking = false;
    }

    private void DeactivateHitCapsuleRef()
    {
        hitCapsuleRef.SetActive(false);
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
        if (states != States.Follow) return;
        agent.isStopped = false;
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

    public override void DeadState()
    {
        base.DeadState();
        if (states != States.Dead) return;
        if (agent.remainingDistance <= 0.5f)
            SelectNextWayPoint();
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
                agent.ResetPath();
                break;
            case States.Dead:
                metamorfosis.IniciarTransicion();
                agent.speed = unCorruptedSpeed;
                ComeBackToTheRoute();
                break;
            default:
                break;
        }
    }

    [ContextMenu("Matar")]
    public void Matar()
    {
        ReceiveDamage(life);
    }

    public void ReceiveDamage(float value)
    {
        if (!corrupted) return;
        life -= value;
        metamorfosis.AplicarEfectoStun();
        if (life <= 0)
        {
            ChangeState(States.Dead);
        }
    }

    public void GetStunned(float duration)
    {
        agent.isStopped = true;
        agent.enabled = false;
        Invoke(nameof(UnStun), duration);
    }

    private void UnStun()
    {
        agent.isStopped = false;
        agent.enabled = true;
    }
}
