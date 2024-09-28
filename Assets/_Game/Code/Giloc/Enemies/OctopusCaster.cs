using Giloc.Attacks;
using Giloc.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giloc.Enemies
{
    public class OctopusCaster : Enemy
    {
        #region properties
        [SerializeField] private List<Cannon> cannons;
        private Transform playerTransform;
        #endregion

        #region unityMethods
        private void Start()
        {
            colliderDecteter.onPlayerDetected += AssignPlayerTransform;
            colliderDecteter.onPlayerExit += IdleMove;
        }
        private void Update()
        {
            WaitNextAttack();
            ChasingMove();
        }

        private void OnDisable()
        {
            colliderDecteter.onPlayerDetected -= AssignPlayerTransform;
            colliderDecteter.onPlayerExit -= IdleMove;
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
                cannon.MakeAttack();
            }
            secondsSinceLastAttack = 0;
            preparingAttack = false;
        }

        protected override void CancelAttack()
        {
            attackCanceled = true;
        }

        protected override void IdleMove()
        {
            playerTransform = null;
        }

        protected override void ChasingMove()
        {
            if (!playerTransform || preparingAttack) return;
            var direction = (playerTransform.position - transform.position).normalized;
            if(Vector3.Distance(transform.position, playerTransform.position) > attackDistance)
            {
                transform.Translate(chasingSpeed * Time.deltaTime * direction);
            }
        }

        protected override IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(attackPreparationTime);
            if(!attackCanceled) Attack();
        }

        protected override void WaitNextAttack()
        {
            if (preparingAttack) return;
            secondsSinceLastAttack += Time.deltaTime;
            if (secondsSinceLastAttack > minTimeBetweenAttacks && !preparingAttack)
            {
                StartCoroutine(PrepareAttack());
                preparingAttack = true;
            }
        }

        private void AssignPlayerTransform(Transform playerTransformDetected)
        {
            playerTransform = playerTransformDetected;
        }
        #endregion
    }
}
