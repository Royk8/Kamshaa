using Giloc.Attacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giloc.Enemies
{
    public class OctopusCaster : Enemy
    {
        #region properties
        [SerializeField] private List<Cannon> cannons;
        private Transform _playerTransform;
        #endregion

        #region unity methods
        private void OnEnable()
        {
            colliderDecteter.onPlayerDetected += StartChasing;
            colliderDecteter.onPlayerExit += StartIdle;
            enemyMovement.OnPlayerReached += AllowAttack;
            foreach (var cannon in cannons)
            {
                cannon.OnAttackFinished += EndAttack;
            }
        }

        private void Start()
        {
            secondsSinceLastAttack = minTimeBetweenAttacks;
        }

        private void Update()
        {
            WaitNextAttack();
        }

        private void OnDisable()
        {
            colliderDecteter.onPlayerDetected -= StartChasing;
            colliderDecteter.onPlayerExit -= StartIdle;
            enemyMovement.OnPlayerReached -= AllowAttack;
            foreach (var cannon in cannons)
            {
                cannon.OnAttackFinished -= EndAttack;
            }
        }
        #endregion

        #region methods
        public override void TakeDamage(float pointsToTake = 1)
        {
            if (!corrupted) return;
            CancelAttack();
            lifePoints -= pointsToTake;

            if (lifePoints <= 0)
            {
                corrupted = false;
                enemyMovement.IsChasing = false;
                playerReached = false;
                StartCoroutine(Die());
            };
        }

        protected override void Attack()
        {
            foreach (var cannon in cannons)
            {
                cannon.MakeAttack(_playerTransform);
            }
        }

        protected override void CancelAttack()
        {
            attackCanceled = true;
            preparingAttack = false;
        }

        protected override IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(attackPreparationTime);
            if (!attackCanceled) Attack();
            else ResetAttackStoppers();
        }

        protected override void WaitNextAttack()
        {
            if (preparingAttack || !playerReached) return;
            secondsSinceLastAttack += Time.deltaTime;
            if (secondsSinceLastAttack > minTimeBetweenAttacks)
            {
                preparingAttack = true;
                StartCoroutine(PrepareAttack());
            }
        }

        private void StartChasing(Transform transform)
        {
            if (!corrupted) return;
            ResetAttackStoppers();
            enemyMovement.StartChasing(transform, attackDistance);
        }

        private void StartIdle()
        {
            playerReached = false;
            secondsSinceLastAttack = minTimeBetweenAttacks;
            CancelAttack();
            enemyMovement.StartIdle();
        }

        private void AllowAttack(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            playerReached = true;
        }

        private void ResetAttackStoppers()
        {
            preparingAttack = false;
            attackCanceled = false;
        }

        private void EndAttack()
        {
            secondsSinceLastAttack = 0;
            preparingAttack = false;
            enemyMovement.ResumeChasing();
        }

        protected override IEnumerator Die()
        {
            yield return null;
            if (metamorfosis != null)
                metamorfosis.IniciarTransicion();
            StartIdle();
        }
        #endregion
    }
}


