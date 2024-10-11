using System.Collections;
using UnityEngine;
using Giloc.Movement;

namespace Giloc
{
    public abstract class Enemy : MonoBehaviour
    {
        #region properties
        [SerializeField] protected int lifePoints;
        [SerializeField] protected float attackPreparationTime;
        [SerializeField] protected float minTimeBetweenAttacks;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected ColliderDetecter colliderDecteter;
        [SerializeField] protected EnemyMovement enemyMovement;
        protected bool attackCanceled;
        protected float secondsSinceLastAttack;
        protected bool preparingAttack;
        protected bool playerReached;
        #endregion

        #region methods
        protected abstract void Attack();
        public abstract void TakeDamage(int pointsToTake = 1);
        protected abstract IEnumerator PrepareAttack();
        protected abstract void CancelAttack();
        protected abstract void WaitNextAttack();
        #endregion
    }
}