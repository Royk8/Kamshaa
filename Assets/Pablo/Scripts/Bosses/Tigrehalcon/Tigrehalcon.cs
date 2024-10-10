using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Rendering.CameraUI;
using static UnityEngine.GraphicsBuffer;

public class Tigrehalcon : MonoBehaviour, IDamageable
{
    private bool actualMechanicIsFinished = false;
    private bool unCorrupted = false;
    private Coroutine mechanicsCoroutine;
    private float normalSpeed;
    public Transform player;
    public float life;
    public NavMeshAgent agent;

    [Space(2)]
    [Header("LaserBeam")]
    [Space(1)]
    public Transform laserBeamPosition;
    public Transform laserBeamStartLookAt, laserBeamEndLookAt;
    public AnimationCurve laserBeamInitialTurnCurve, laserBeamTurnCurve;
    public GameObject laserBeam;
    public float laserBeamInitialTurnSpeed;
    public float laserBeamTurnSpeed;
    public float laserBeamChargeSpeed;
    private bool alreadyTurned;

    [Space(2)]
    [Header("persecution")]
    [Space(1)]
    public GameObject basicAttackHitSphere;
    public LayerMask hitMask;
    public float persecutionDamage;
    public float persecutionRangeAttack;
    public float persecutionAttackFrecuence;
    public float persecutionDuration;

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
        }
    }

    private IEnumerator LaserBeam()
    {
        agent.speed = laserBeamChargeSpeed;
        agent.SetDestination(laserBeamPosition.position);
        yield return new WaitUntil(() => agent.remainingDistance <= 0.5f);

        Vector3 direction = (laserBeamStartLookAt.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation, laserBeamInitialTurnSpeed, laserBeamInitialTurnCurve));
        yield return new WaitUntil(() => alreadyTurned);

        laserBeam.SetActive(true);
        direction = (laserBeamEndLookAt.position - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        alreadyTurned = false;
        StartCoroutine(TurnToTarget(targetRotation, laserBeamTurnSpeed, laserBeamTurnCurve));
        yield return new WaitUntil(() => alreadyTurned);

        agent.speed = normalSpeed;
        laserBeam.SetActive(false);
        actualMechanicIsFinished = true;
    }

    private IEnumerator TurnToTarget(Quaternion targetRotation, float turnVelocity, AnimationCurve curve)
    {
        Quaternion initialRotation = transform.rotation;
        float currentCurveValue = 0;

        while (currentCurveValue != 1)
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
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator BasicAttackBucle()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            basicAttackHitSphere.SetActive(false);
            if ((transform.position - player.position).magnitude > persecutionRangeAttack) continue;

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
        if (collidersAffected.Length > 0)
            for (int i = 0; i < collidersAffected.Length; i++)
            {
                if (collidersAffected[i].gameObject == gameObject) continue;
                Debug.Log($"{collidersAffected[i].name} afectado por ataque basico de: {transform.name}");

                if (collidersAffected[i].TryGetComponent(out IDamageable damageable))
                {
                    damageable.ReceiveDamage(damage);
                    Debug.Log($"{collidersAffected[i].name} dañado por ataque basico de: {transform.name}");
                }
            }
    }

    public void ReceiveDamage(float value)
    {
        if (unCorrupted) return;

        life -= value;
        if (life <= 0)
        {
            unCorrupted = true;
            agent.speed = normalSpeed;
            ControlAmbiente.singleton.LlenarRojo();
        }
    }
}
