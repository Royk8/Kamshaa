using System.Collections;
using UnityEngine;
using Giloc.Enums;

namespace Giloc
{
    public abstract class Enemy : MonoBehaviour
    {
        #region properties
        [SerializeField] protected int lifePoints;
        [SerializeField] protected float attackPreparationTime;
        [SerializeField] protected float minTimeBetweenAttacks;
        [SerializeField] protected float chasingSpeed;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected ColliderDetecter colliderDecteter;
        protected bool attackCanceled;
        protected float secondsSinceLastAttack;
        protected bool preparingAttack;
        #endregion

        #region methods
        protected abstract void Attack();
        public abstract void TakeDamage(int pointsToTake = 1);
        protected abstract void IdleMove();
        protected abstract void ChasingMove();
        protected abstract IEnumerator PrepareAttack();
        protected abstract void CancelAttack();
        protected abstract void WaitNextAttack();
        #endregion
    }
}