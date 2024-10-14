using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Cucarron : Enemy, IDamageable, IStuneable
{
    public NavMeshAgent agent;
    public Animator anim;
    public AnimationClip movementClip;
    public bool readyToMove = true;
    public float chargeDuration;
    public float chargeForce;
    public float damage;
    public AnimationCurve turnCurveVelocity;
    public float turnVelocity;
    public List<Transform> wanderingPoints;
    public Metamorfosis metamorfosis;
    public Collider cucarronCollider;
    public UnityEvent iniciaCaminar;
    public UnityEvent iniciaCorrer;
    public UnityEvent terminaCaminar;
    public UnityEvent terminaCorrer;

    private Transform actualDestination;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool buried = true;
    private bool alreadyTurned = false;
    private FMOD.Studio.EventInstance walking;

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
                terminaCorrer.Invoke();
                iniciaCaminar.Invoke();
                metamorfosis.IniciarTransicion();
                SelectNextWayPoint();
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
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.Yahtu1Attack, this.transform.position);
        terminaCaminar.Invoke();
        iniciaCorrer.Invoke();
        float normalVelocity = agent.speed;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation));
        yield return new WaitUntil(() => alreadyTurned);
        isCharging = true;
        agent.speed = chargeForce;
        agent.SetDestination(target.position + (transform.forward * 1.4f));
        yield return new WaitForSeconds(chargeDuration);
        isCharging = false;
        terminaCorrer.Invoke();
        iniciaCaminar.Invoke();
        agent.speed = normalVelocity;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCharging) return;
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.ReceiveDamage(damage);
        }
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
            iniciaCaminar.Invoke();
            walking = AudioManager.Instance.NuevaInstancia(EventsManager.Instance.Yahtu1Walking);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(walking, GetComponent<Transform>(), GetComponent<Rigidbody>());
            walking.start();
            cucarronCollider.enabled = true;
            agent.SetDestination(target.position);
        }
    }

    public override void IdleState()
    {
        base.IdleState();
        if (buried) return;
        if ((transform.position - initalPosition).magnitude < 0.7f)
        {
            terminaCaminar.Invoke();
            walking.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            cucarronCollider.enabled = false;
            readyToMove = false;
            anim.SetBool("IsMoving", false);
            buried = true;
        }
    }

    public override void DeadState()
    {
        base.DeadState();
        if (agent.remainingDistance <= 0.5f)
            SelectNextWayPoint();
    }

    private void SelectNextWayPoint()
    {
        int nextDestinationIndex = wanderingPoints.IndexOf(actualDestination) == (wanderingPoints.Count - 1) ? 0 : wanderingPoints.IndexOf(actualDestination) + 1;
        actualDestination = wanderingPoints[nextDestinationIndex];
        agent.SetDestination(actualDestination.position);
    }

    //this is called from an animation event in the animation Exit_Idle
    public void IsReadyToMove()
    {
        readyToMove = true;
    }

    [ContextMenu("matar")]
    private void Matar()
    {
        ReceiveDamage(life);
    }

    public void ReceiveDamage(float value)
    {
        if (!corrupted) return;
        life -= value;
        if (life <= 0)
        {
            ChangeState(States.Dead);
        }
        else
        {
            metamorfosis.AplicarEfectoStun();
        }
        AudioManager.Instance.PlayOneShot(EventsManager.Instance.Yahtu1Hurt, this.transform.position);
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
