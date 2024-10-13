using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Mantis : MonoBehaviour, IDamageable
{
    private bool actualMechanicIsFinished = false;
    private bool unCorrupted = false;
    private Coroutine mechanicsCoroutine;
    public Transform player;
    public VFXControlMantis vfxAnim;
    public float tpDelay;
    public float maxLife;
    public float life;
    public Metamorfosis metamorfosis;
    public Slider healthBar;
    public MonolitosVerdes monolitosVerdes;

    [Space(2)]
    [Header("Basic Attack")]
    [Space(1)]
    public float basicAttackRange = 3;
    public float basicAttackFrecuence = 1;
    public float basicAttackDamage = 1;
    public GameObject basicAttackHitSphere;
    [Space(2)]
    [Header("Slash")]
    [Space(1)]
    public List<Transform> slashUbications = new();
    private List<Transform> slashUbicationsRemaining = new();
    public List<GameObject> hitBoxIndicators;
    public VFXControlMantis phantomMantis;
    public float phantomMantisSpeed;
    public AnimationCurve phantomMantisSpeedCurve;
    public Material warningMaterial, hitMaterial;
    public LayerMask hitBoxMask;
    public string actualMechanic;
    public float slashDamage, slashWait, slashWarningDelay, slashFinishDelay;
    private Transform selectedPosition;
    [Space(2)]
    [Header("Poison")]
    [Space(1)]
    public NavMeshAgent agent;
    public List<Transform> posiblePositions = new();
    private List<Transform> posiblePositionsRemaining = new();
    public PoisonTick poisonTick;
    private Transform spawnPosition;
    [Space(2)]
    [Header("DeadlyOmen")]
    [Space(1)]
    public float dOStunDuration = 1.5f;
    public float timeUntilDO = 1f;
    public float dODamage = 8f;
    public float speedDuringDO, afterDeadlyOmenDelay;
    public Transform mantisDOSpawnPosition;
    public Transform playerDOSpawnPosition;
    [Space(2)]
    [Header("Wandering")]
    [Space(1)]
    public float minWanderingTime;
    public float maxWanderingTime;
    private Transform actualDestination;

    private void Start()
    {
        healthBar.maxValue = life;
        healthBar.value = life;
        healthBar.transform.parent.gameObject.SetActive(true);
        mechanicsCoroutine = StartCoroutine(MechanicsController());
    }

    private IEnumerator MechanicsController()
    {
        while (true)
        {
            actualMechanicIsFinished = false;
            StartCoroutine(Slash());
            actualMechanic = nameof(Slash);
            yield return new WaitUntil(() => actualMechanicIsFinished);
            actualMechanicIsFinished = false;
            StartCoroutine(Poison());
            actualMechanic = nameof(Poison);
            yield return new WaitUntil(() => actualMechanicIsFinished);
            actualMechanicIsFinished = false;
            StartCoroutine(DeadlyOmen());
            actualMechanic = nameof(DeadlyOmen);
            yield return new WaitUntil(() => actualMechanicIsFinished);
        }
    }

    private IEnumerator Slash()
    {
        agent.enabled = false;
        if (slashUbicationsRemaining.Count == 0)
        {
            slashUbicationsRemaining = slashUbications.Where(x => x).ToList();
            slashUbicationsRemaining.Remove(selectedPosition);
        }

        // se posiciona la mantis en el lugar de donde va a atacar
        selectedPosition = slashUbicationsRemaining[Random.Range(0, slashUbicationsRemaining.Count)];
        slashUbicationsRemaining.Remove(selectedPosition);
        vfxAnim.Invisibilizar();
        yield return new WaitForSeconds(tpDelay);
        transform.SetPositionAndRotation(selectedPosition.position, selectedPosition.rotation);
        vfxAnim.Visibilizar();
        yield return new WaitForSeconds(tpDelay);
        yield return new WaitForSeconds(slashWait);

        // mostramos el area donde va a golpear
        int randomArea = Random.Range(0, hitBoxIndicators.Count);
        hitBoxIndicators[randomArea].GetComponent<MeshRenderer>().material = warningMaterial;
        hitBoxIndicators[randomArea].SetActive(true);
        phantomMantis.transform.position = hitBoxIndicators[randomArea].transform.GetChild(0).position;
        phantomMantis.gameObject.SetActive(true);
        phantomMantis.Visibilizar();
        yield return new WaitForSeconds(slashWarningDelay);

        // crea las hitbox donde va a golpear
        StartCoroutine(MovePhantomMantis(hitBoxIndicators[randomArea].transform.GetChild(1).position));
        hitBoxIndicators[randomArea].GetComponent<MeshRenderer>().material = hitMaterial;
        Collider[] collidersAffected = Physics.OverlapBox(hitBoxIndicators[randomArea].transform.position, hitBoxIndicators[randomArea].transform.localScale / 2, hitBoxIndicators[randomArea].transform.rotation, hitBoxMask);

        for (int i = 0; i < collidersAffected.Length; i++)
        {
            if (collidersAffected[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.ReceiveDamage(slashDamage);
                Debug.Log($"{collidersAffected[i].name} dañado por slash de: {transform.name} ");
            }
            else
                Debug.Log($"{collidersAffected[i].name} affected for slash");

        }
        yield return new WaitForSeconds(slashFinishDelay);

        phantomMantis.gameObject.SetActive(false);
        hitBoxIndicators[randomArea].SetActive(false);
        actualMechanicIsFinished = true;
    }

    private IEnumerator MovePhantomMantis(Vector3 targetPosition)
    {
        Vector3 initialPosition = phantomMantis.transform.position;
        float currentCurveValue = 0;
        phantomMantis.IniciarAtaqueLargo();

        while (currentCurveValue != 1)
        {
            currentCurveValue = Mathf.MoveTowards(currentCurveValue, 1, phantomMantisSpeed * Time.fixedDeltaTime);

            phantomMantis.transform.position = Vector3.Lerp(initialPosition, targetPosition, phantomMantisSpeedCurve.Evaluate(currentCurveValue));
            yield return new WaitForFixedUpdate();
        }
        phantomMantis.TerminarAtaqueLargo();
        phantomMantis.Invisibilizar();
    }

    private IEnumerator Poison()
    {
        if (posiblePositionsRemaining.Count == 0)
        {
            posiblePositionsRemaining = posiblePositions.Where(x => x).ToList();
            posiblePositionsRemaining.Remove(spawnPosition);
        }

        // posiciona a la mantis en una de las posibles posiciones de spawn para ir a atacar al jugador
        spawnPosition = posiblePositionsRemaining[Random.Range(0, posiblePositionsRemaining.Count)];
        posiblePositionsRemaining.Remove(spawnPosition);
        vfxAnim.Invisibilizar();
        yield return new WaitForSeconds(tpDelay);
        transform.SetPositionAndRotation(spawnPosition.position, spawnPosition.rotation);
        vfxAnim.Visibilizar();
        yield return new WaitForSeconds(tpDelay);

        // hace que la mantis persiga al jugador
        agent.enabled = true;
        agent.isStopped = false;
        Coroutine destinationCo = StartCoroutine(SetMantisDestination(player.transform));
        Coroutine basicAttackCo = StartCoroutine(BasicAttackBucle());

        // ubica y activa la nube de veneno
        poisonTick.transform.position = new Vector3 (player.transform.position.x, poisonTick.transform.position.y, player.transform.position.z);
        poisonTick.gameObject.SetActive(true);
        yield return new WaitForSeconds(poisonTick.poisonWarning + poisonTick.poisonDuration);
        poisonTick.gameObject.SetActive(false);
        StopCoroutine(destinationCo);
        StopCoroutine(basicAttackCo);
        basicAttackHitSphere.SetActive(false);
        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;
        actualMechanicIsFinished = true;
    }

    private IEnumerator SetMantisDestination(Transform destination)
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
            if ((transform.position - player.position).magnitude > basicAttackRange) continue;

            StartCoroutine(BasicAttack(basicAttackDamage));

            yield return new WaitForSeconds(basicAttackFrecuence);
        }
    }

    private IEnumerator BasicAttack(float damage)
    {
        vfxAnim.Golpear();
        yield return new WaitForSeconds(0.6f);
        Collider[] collidersAffected = Physics.OverlapSphere(basicAttackHitSphere.transform.position, basicAttackHitSphere.transform.localScale.x / 2, hitBoxMask);
        basicAttackHitSphere.SetActive(true);
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

    private IEnumerator DeadlyOmen()
    {
        float normalSpeed = agent.speed;
        vfxAnim.Invisibilizar();
        yield return new WaitForSeconds(tpDelay);
        transform.SetPositionAndRotation(mantisDOSpawnPosition.position, mantisDOSpawnPosition.rotation);
        vfxAnim.Visibilizar();
        yield return new WaitForSeconds(tpDelay);

        if (player.TryGetComponent(out IStuneable stuneable))
        {
            stuneable.GetStunned(dOStunDuration);
        }
        else
            Debug.Log("no se encuentra el script con la interfaz de IStuneable en el player");

        player.SetPositionAndRotation(playerDOSpawnPosition.position, playerDOSpawnPosition.rotation);
        agent.speed = speedDuringDO;
        agent.enabled = true;
        agent.SetDestination(player.position + (transform.forward * 5));

        yield return new WaitForSeconds(timeUntilDO);
        StartCoroutine(BasicAttack(dODamage));
        yield return new WaitForSeconds(afterDeadlyOmenDelay);
        agent.speed = normalSpeed;
        agent.ResetPath();
        agent.enabled = false;
        basicAttackHitSphere.SetActive(false);
        actualMechanicIsFinished = true;
    }

    private IEnumerator Wandering()
    {
        agent.isStopped = false;
        agent.enabled = true;
        agent.speed = speedDuringDO;
        while (true)
        {
            actualDestination = posiblePositions.Where(x => x != actualDestination).ToList()[Random.Range(0, posiblePositions.Count - 1)];
            agent.SetDestination(actualDestination.position);
            yield return new WaitForSeconds(Random.Range(minWanderingTime, maxWanderingTime));
        }
    }

    [ContextMenu("Matar")]
    public void Matar()
    {
        ReceiveDamage(life);
    }

    public void ReceiveDamage(float value)
    {
        if (unCorrupted) return;

        life -= value;
        healthBar.value = life;
        if (life <= 0)
        {
            metamorfosis.IniciarTransicion();
            unCorrupted = true;
            StopAllCoroutines();
            mechanicsCoroutine = StartCoroutine(Wandering());
            actualMechanic = nameof(Wandering);
            poisonTick.gameObject.SetActive(false);
            monolitosVerdes.VolverConVibracion();
            Plumero.singleton.AdquirirPluma(Pluma.verde);
            ControlAmbiente.singleton.LlenarVerde();
            healthBar.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            metamorfosis.AplicarEfectoStun();
        }
    }

    public void Heal(float value)
    {
        life += value;
        healthBar.value = life;
        if (life > maxLife)
            life = maxLife;
    }
}