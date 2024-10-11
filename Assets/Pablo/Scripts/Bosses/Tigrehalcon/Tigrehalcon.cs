using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Tigrehalcon : MonoBehaviour, IDamageable
{
    private bool actualMechanicIsFinished = false;
    private bool unCorrupted = false;
    private Coroutine mechanicsCoroutine;
    private float normalSpeed;
    private Transform actualDestination;
    public Transform player;
    public float life;
    public float minWanderingTime, maxWanderingTime;
    public NavMeshAgent agent;
    public Collider _collider;
    public List<Transform> wanderingPoints = new();
    public float wanderingSpeed;
    public Animator anim;
    public VFXControlBossRojo vfx;

    [Space(2)]
    [Header("LaserBeam")]
    [Space(1)]
    public Transform laserBeamPosition;
    public Transform laserBeamLookAtOne, laserBeamLookAtTwo;
    private Transform laserBeamLookAtFirst, laserBeamLookAtFinal;
    public AnimationCurve laserBeamInitialTurnCurve, laserBeamTurnCurve;
    public GameObject laserBeam;
    public float laserBeamInitialTurnSpeed;
    public float laserBeamAnimDelay;
    public float laserBeamTurnSpeed;
    public float chargeSpeed;
    public float stunTime;
    private bool alreadyTurned;
    private bool isCharging;

    [Space(2)]
    [Header("Persecution")]
    [Space(1)]
    public GameObject basicAttackHitSphere;
    public LayerMask hitMask;
    public float persecutionDamage;
    public float persecutionLongRangeAttack;
    public float persecutionShortRangeAttack;
    public float persecutionAttackFrecuence;
    public float persecutionDuration;
    public float basicAttackTurnVelocity;

    [Space(2)]
    [Header("FirePillars")]
    [Space(1)]
    public List<GameObject> pillarRails = new();
    public Transform pillarsCastPosition;
    public float pillarsDuration;
    private GameObject actualSkipedPillarRail;

    private void Awake()
    {
        normalSpeed = agent.speed;
    }

    private void Start()
    {
        StartCoroutine(MechanicsController());
    }

    private IEnumerator MechanicsController()
    {
        while (true)
        {
            actualMechanicIsFinished = false;
            StartCoroutine(LaserBeam());
            yield return new WaitUntil(() => actualMechanicIsFinished);
            actualMechanicIsFinished = false;
            StartCoroutine(Persecution());
            yield return new WaitUntil(() => actualMechanicIsFinished);
            actualMechanicIsFinished = false;
            StartCoroutine(PillarsOfFire());
            yield return new WaitUntil(() => actualMechanicIsFinished);
        }
    }

    private IEnumerator LaserBeam()
    {
        agent.speed = chargeSpeed;
        agent.isStopped = false;
        agent.SetDestination(laserBeamPosition.position);
        yield return new WaitForSeconds(0.5f);
        isCharging = true;
        anim.SetBool("corriendo", isCharging);
        vfx.VerParticulas(isCharging);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);
        anim.SetBool("corriendo", false);

        if ((laserBeamLookAtTwo.position - player.position).magnitude < (laserBeamLookAtOne.position - player.position).magnitude)
        {
            laserBeamLookAtFirst = laserBeamLookAtTwo;
            laserBeamLookAtFinal = laserBeamLookAtOne;
        }
        else
        {
            laserBeamLookAtFirst = laserBeamLookAtOne;
            laserBeamLookAtFinal = laserBeamLookAtTwo;
        }

        Vector3 direction = (laserBeamLookAtFirst.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation, laserBeamInitialTurnSpeed, laserBeamInitialTurnCurve));
        yield return new WaitUntil(() => alreadyTurned);

        anim.SetBool("poder", true);
        yield return new WaitForSeconds(laserBeamAnimDelay);

        ControladorCamara.singleton.IniciarTemblor(3, 0.06f);
        laserBeam.SetActive(true);
        direction = (laserBeamLookAtFinal.position - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation, laserBeamTurnSpeed, laserBeamTurnCurve));
        yield return new WaitUntil(() => alreadyTurned);

        isCharging = false;
        vfx.VerParticulas(isCharging);
        agent.speed = normalSpeed;
        laserBeam.SetActive(false);
        anim.SetBool("poder", false);
        actualMechanicIsFinished = true;
    }

    private IEnumerator TurnToTarget(Quaternion targetRotation, float turnVelocity, AnimationCurve curve)
    {
        Quaternion initialRotation = transform.rotation;
        float currentCurveValue = 0;

        while (currentCurveValue < 1f)
        {
            currentCurveValue = Mathf.MoveTowards(currentCurveValue, 1, turnVelocity * Time.fixedDeltaTime);

            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, curve.Evaluate(currentCurveValue));
            yield return new WaitForFixedUpdate();
        }
        alreadyTurned = true;
    }

    private IEnumerator Persecution()
    {
        Coroutine SetDestinationCoroutine = StartCoroutine(SetTigrehalconDestination(player));
        Coroutine BasicAttackCoroutine = StartCoroutine(BasicAttackBucle());
        yield return new WaitForSeconds(persecutionDuration);

        StopCoroutine(SetDestinationCoroutine);
        StopCoroutine(BasicAttackCoroutine);
        actualMechanicIsFinished = true;
    }

    private IEnumerator SetTigrehalconDestination(Transform destination)
    {
        while (true)
        {
            agent.SetDestination(destination.position);
            if (agent.remainingDistance < persecutionShortRangeAttack)
                agent.isStopped = true;
            else
                agent.isStopped = false;

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator BasicAttackBucle()
    {
        while (true)
        {
            basicAttackHitSphere.SetActive(false);
            if ((transform.position - player.position).magnitude > persecutionLongRangeAttack)
            {
                yield return new WaitForSeconds(0.3f);
                continue;
            }

            if ((transform.position - player.position).magnitude <= persecutionShortRangeAttack)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                alreadyTurned = false;
                StartCoroutine(TurnToTarget(targetRotation, basicAttackTurnVelocity, laserBeamInitialTurnCurve));
                yield return new WaitUntil(() => alreadyTurned);
            }

            StartCoroutine(BasicAttack(persecutionDamage));

            yield return new WaitForSeconds(persecutionAttackFrecuence);
        }
    }

    private IEnumerator BasicAttack(float damage)
    {
        //vfxAnim.Golpear();
        yield return new WaitForSeconds(0.6f);
        Collider[] collidersAffected = Physics.OverlapSphere(basicAttackHitSphere.transform.position, basicAttackHitSphere.transform.localScale.x / 2, hitMask);
        basicAttackHitSphere.SetActive(true);
        Invoke(nameof(DeactivateHitCollider), 0.5f);
        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i].gameObject == gameObject) continue;

            if (collidersAffected[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.ReceiveDamage(damage);
                Debug.Log($"{collidersAffected[i].name} dañado por ataque basico de: {transform.name}");
            }
        }
    }

    private IEnumerator PillarsOfFire()
    {
        agent.speed = chargeSpeed;
        agent.isStopped = false;
        agent.SetDestination(pillarsCastPosition.position);
        yield return new WaitForSeconds(0.5f);
        isCharging = true;
        anim.SetBool("corriendo", isCharging);
        vfx.VerParticulas(isCharging);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.25f);

        anim.SetBool("corriendo", false);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(pillarsCastPosition.rotation, laserBeamInitialTurnSpeed, laserBeamInitialTurnCurve));
        yield return new WaitUntil(() => alreadyTurned);

        anim.SetBool("poder", true);
        actualSkipedPillarRail = pillarRails.Where(x => x != actualSkipedPillarRail).ToList()[Random.Range(0, pillarRails.Count - 1)];
        List<GameObject> actualPillarRails = pillarRails.Where(x => x != actualSkipedPillarRail).ToList();
        for (int i = 0; i < actualPillarRails.Count; i++)
        {
            actualPillarRails[i].SetActive(true);
        }

        yield return new WaitForSeconds(pillarsDuration);

        anim.SetBool("poder", false);
        isCharging = false;
        vfx.VerParticulas(isCharging);
        for (int i = 0; i < actualPillarRails.Count; i++)
        {
            actualPillarRails[i].GetComponent<FirePillarRail>().StartDeactivate();
        }
        actualMechanicIsFinished = true;
    }

    private void DeactivateHitCollider()
    {
        basicAttackHitSphere.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCharging) return;
        if (other.TryGetComponent(out IStuneable stuneable))
        {
            stuneable.GetStunned(stunTime);
        }
    }

    private IEnumerator Wandering()
    {
        agent.speed = wanderingSpeed;
        agent.isStopped = false;
        while (true)
        {
            actualDestination = wanderingPoints.Where(x => x != actualDestination).ToList()[Random.Range(0, wanderingPoints.Count - 1)];
            agent.SetDestination(actualDestination.position);
            yield return new WaitForSeconds(Random.Range(minWanderingTime, maxWanderingTime));
        }
    }

    [ContextMenu("Matar")]
    private void Matar()
    {
        ReceiveDamage(life);
    }

    public void ReceiveDamage(float value)
    {
        if (unCorrupted) return;

        life -= value;
        if (life <= 0)
        {
            unCorrupted = true;
            StopAllCoroutines();
            StartCoroutine(Wandering());
            ControlAmbiente.singleton.LlenarRojo();
        }
    }
}
