using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class Vespa : Enemy, IEnemy
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public AnimationCurve turnCurveVelocity;
    public float turnVelocity;
    public float shootingRate;
    public List<GameObject> stings = new();
    public GameObject stingPrefab;
    public Transform shootingSpot;

    private Transform actualPoint;
    private bool isAttacking;
    private bool alreadyTurned = false;

    private void Start()
    {
        SelectNextWayPoint();
    }

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

        yield return new WaitUntil(() => alreadyTurned);

        GameObject actualSting = stings.Where(x => !x.activeSelf).FirstOrDefault();
        if (actualSting == null)
        {
            actualSting = Instantiate(stingPrefab, shootingSpot.position, Quaternion.identity);
            actualSting.GetComponent<Sting>().creator = gameObject;
            stings.Add(actualSting);
        }
        actualSting.transform.position = shootingSpot.position;
        actualSting.transform.LookAt(target.position + new Vector3(0, 1, 0));
        actualSting.SetActive(true);

        yield return new WaitForSeconds(shootingRate);

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
        if (states != States.Follow) return;
        agent.isStopped = false;
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

        agent.isStopped = false;
        agent.SetDestination(actualPoint.position);
    }

    public override void IdleState()
    {
        base.IdleState();
        if (states != States.Idle) return;
        if (agent.remainingDistance <= 0.5f)
            SelectNextWayPoint();
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
                break;
            default:
                break;
        }
    }

    public void ReceiveDamage(int damageDealed)
    {
    }

    public IEnumerator GetStuned(float timeStuned)
    {
        yield return null;
    }
}
