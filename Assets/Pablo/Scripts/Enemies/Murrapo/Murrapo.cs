using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Murrapo : Enemy, IDamageable
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public GameObject hitSphere;
    public AnimationCurve turnCurveVelocity;
    public AnimationCurve jumpCurveVelocity;
    public AnimationCurve positionCurveVelocity;
    public LayerMask mask;
    public float attackColdDown;
    public float temblorDuration;
    public float temblorMagnitud;
    public float turnVelocity;
    public float jumpVelocity;
    public float highTarget;
    public float damage;
    public float wanderingSpeed;

    private Transform actualPoint;
    private bool isAttacking;
    private bool jumpLanded;
    private bool alreadyTurned = false;

    public override void IdleState()
    {
        base.IdleState();
        if (states != States.Idle) return;
        if (!agent.enabled) return;
        if (agent.remainingDistance <= 0.5f)
            SelectNextWayPoint();
    }

    public override void FollowState()
    {
        base.FollowState();
        if (states != States.Follow) return;
        if (!agent.enabled) return;
        agent.isStopped = false;
        if (agent.enabled)
            agent.SetDestination(target.position);
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

        agent.enabled = true;
        agent.isStopped = false;
        agent.SetDestination(actualPoint.position);
    }

    public override void AttackingState()
    {
        base.AttackingState();
        if (states != States.Attacking) return;
        if (agent.enabled)
        agent.isStopped = false;
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
        yield return new WaitUntil(() => alreadyTurned);

        jumpLanded = false;
        StartCoroutine(Jump(target.position, highTarget));
        yield return new WaitUntil(() => jumpLanded);

        isAttacking = false;
    }

    private IEnumerator Jump(Vector3 targetPosition, float targetYPosition)
    {
        Vector3 initialPosition = transform.position;
        float initialYPosition = transform.position.y;
        float currentCurveValue = 0;
        agent.enabled = false;

        while (currentCurveValue != 1)
        {
            currentCurveValue = Mathf.MoveTowards(currentCurveValue, 1, jumpVelocity * Time.fixedDeltaTime);

            Vector3 zPosition = Vector3.Lerp(initialPosition, targetPosition, positionCurveVelocity.Evaluate(currentCurveValue));
            float yPosition = math.lerp(initialYPosition, targetYPosition, jumpCurveVelocity.Evaluate(currentCurveValue));
            transform.position = new Vector3(zPosition.x, yPosition, zPosition.z);
            yield return new WaitForFixedUpdate();
        }
        DamageHit();

        yield return new WaitForSeconds(attackColdDown);

        agent.enabled = true;
        jumpLanded = true;
    }

    private void DamageHit()
    {
        hitSphere.SetActive(true);
        ControladorCamara.singleton.IniciarTemblor(temblorDuration, temblorMagnitud);
        Collider[] collidersAffected = Physics.OverlapSphere(hitSphere.transform.position, hitSphere.transform.localScale.x / 2, mask);

        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i] == GetComponent<Collider>()) continue;
            if (collidersAffected[i].TryGetComponent(out IDamageable creature))
            {
                creature.ReceiveDamage(damage);
                Debug.Log(collidersAffected[i].name + " dañado por poison de: " + name);
            }
        }
        Invoke(nameof(DeactivateHitSphere), 0.2f);
    }

    private void DeactivateHitSphere()
    {
        hitSphere.SetActive(false);
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
                break;
            case States.Dead:
                ComeBackToTheRoute();
                agent.speed = wanderingSpeed;
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
