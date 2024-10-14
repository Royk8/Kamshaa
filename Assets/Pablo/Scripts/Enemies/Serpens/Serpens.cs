using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;

public class Serpens : Enemy, IDamageable
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public AnimationCurve turnCurveVelocity;
    public LayerMask mask;
    public GameObject hitSphere;
    public Animator anim;
    public Metamorfosis metamorfosis;
    public float turnVelocity;
    public float wanderingSpeed;
    public float attackSpeed;
    public float attackRange;
    public float animAttackDuration;
    public float damage;
    public float unitsBackUp, unitsLeft, UnitsRight;

    private Transform actualPoint;
    private bool isAttacking;
    private Coroutine followPlayerCo;
    private Coroutine turnToTargetCo;
    private Coroutine attackCo;

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

        agent.SetDestination(actualPoint.position);
    }

    public override void AttackingState()
    {
        base.AttackingState();
        if (states != States.Attacking) return;
        if (isAttacking) return;

        isAttacking = true;
        attackCo = StartCoroutine(Attack());
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

    private IEnumerator FollowPlayerConstantly()
    {
        while (true)
        {
            agent.SetDestination(target.position);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Attack()
    {
        agent.speed = attackSpeed;
        
        turnToTargetCo = StartCoroutine(TurnToTarget());
        followPlayerCo = StartCoroutine(FollowPlayerConstantly());

        yield return new WaitForSeconds(0.4f);
        yield return new WaitUntil(() => agent.remainingDistance <= attackRange);

        anim.SetTrigger("atacar");
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.Teshemba2Attack, this.transform.position);
        yield return new WaitForSeconds(animAttackDuration);

        StopCoroutine(followPlayerCo);
        DamageHit();
        Vector3 destination = transform.position + (transform.forward * unitsBackUp);
        agent.SetDestination(destination);
        yield return new WaitForSeconds(0.4f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);

        destination = transform.position + (transform.right * UnitsRight);
        agent.SetDestination(destination);
        yield return new WaitForSeconds(0.4f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);

        destination = transform.position + (transform.right * -unitsLeft);
        agent.SetDestination(destination);
        yield return new WaitForSeconds(0.4f);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);

        StopCoroutine(turnToTargetCo);
        isAttacking = false;
    }

    private void StopAttackingCoroutines()
    {
        if (attackCo != null)
        StopCoroutine(attackCo);
        if (turnToTargetCo != null)
        StopCoroutine(turnToTargetCo);
        if (followPlayerCo != null)
        StopCoroutine(followPlayerCo);
        isAttacking = false;
    }

    private void DamageHit()
    {
        hitSphere.SetActive(true);
        Collider[] collidersAffected = Physics.OverlapSphere(hitSphere.transform.position, hitSphere.transform.localScale.x / 2, mask);

        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i] == GetComponent<Collider>()) continue;
            if (collidersAffected[i].TryGetComponent(out IDamageable creature))
            {
                creature.ReceiveDamage(damage);
                Debug.Log(collidersAffected[i].name + " dañado por ataque de: " + name);
            }
        }
        Invoke(nameof(DeactivateHitSphere), 0.2f);
    }

    private void DeactivateHitSphere()
    {
        hitSphere.SetActive(false);
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
                StopAttackingCoroutines();
                ComeBackToTheRoute();
                break;
            case States.Follow:
                StopAttackingCoroutines();
                break;
            case States.Attacking:
                break;
            case States.Dead:
                StopAttackingCoroutines();
                agent.speed = wanderingSpeed;
                metamorfosis.IniciarTransicion();
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
        metamorfosis.AplicarEfectoStun();
        if (life <= 0)
        {
            ChangeState(States.Dead);
        }
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.Teshemba2Hurt, this.transform.position);
    }
}
