using Giloc.Attacks;
using System.Collections;
using UnityEngine;

namespace Giloc.Enemies
{
    public class OctopusMelee : Enemy
    {
        #region properties
        [SerializeField] private Tentacle tentacle;
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
            throw new System.NotImplementedException();
        }

        protected override void Attack()
        {
            throw new System.NotImplementedException();
        }

        protected override void CancelAttack()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator PrepareAttack()
        {
            throw new System.NotImplementedException();
        }

        protected override void WaitNextAttack()
        {
            throw new System.NotImplementedException();
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

        private void AllowAttack()
        {
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


