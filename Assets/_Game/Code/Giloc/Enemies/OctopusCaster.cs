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
        }
        #endregion

        #region methods
        public override void TakeDamage(int pointsToTake = 1)
        {
            CancelAttack();
            lifePoints -= pointsToTake;
        }

        protected override void Attack()
        {
            foreach (var cannon in cannons)
            {
                cannon.MakeAttack(_playerTransform);
            }
            secondsSinceLastAttack = 0;
            preparingAttack = false;
            enemyMovement.ResumeChasing();
        }

        protected override void CancelAttack()
        {
            attackCanceled = true;
            preparingAttack = false;
        }

        protected override IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(attackPreparationTime);
            preparingAttack = false;
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
        #endregion
    }
}


