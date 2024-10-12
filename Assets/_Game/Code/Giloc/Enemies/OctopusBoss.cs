using Giloc.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giloc.Enemies
{
    public class OctopusBoss : Enemy
    {
        #region properties
        [SerializeField] private List<Cannon> cannons;
        [SerializeField] private Cannon parabolicCannon;
        private bool _isRotating;
        private float _rotationAmount;
        private float _rotationSpeed;
        private Quaternion _startRotation;
        private Transform _playerTransform;
        private float _originalLife;
        private int attackType = 0;
        #endregion

        #region unityMethods
        private void OnEnable()
        {
            colliderDecteter.onPlayerDetected += StartChasing;
            colliderDecteter.onPlayerExit += StartIdle;
            enemyMovement.OnPlayerReached += AllowAttack;
            foreach (var cannon in cannons)
            {
                cannon.OnAttackFinished += EndAttack;
            }
            parabolicCannon.OnAttackFinished += EndAttack;
        }

        private void Start()
        {
            secondsSinceLastAttack = minTimeBetweenAttacks;
            _rotationSpeed = 360f / (attackPreparationTime * 5);
            _originalLife = lifePoints;
        }

        private void Update()
        {
            WaitNextAttack();
            RotateWhilePreparingAttack();
            LookAtPLayer();
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
            parabolicCannon.OnAttackFinished -= EndAttack;
        }
        #endregion

        #region methods
        public override void TakeDamage(float pointsToTake = 1)
        {
            if (!corrupted) return;
            CancelAttack();
            lifePoints -= pointsToTake;

            if(lifePoints <= 0)
            {
                enemyMovement.IsChasing = false;
                corrupted = false;
                playerReached = false;
                StartCoroutine(Die());
            }
        }

        protected override void Attack()
        {
            if(lifePoints < _originalLife / 2)
            {
                attackType = UnityEngine.Random.Range(0, 2);
            }
            if(attackType == 0)
            {
                foreach (var cannon in cannons)
                {
                    cannon.MakeAttack();
                }
            }
            else
            {
                parabolicCannon.MakeAttack(_playerTransform);
            }
        }

        protected override void CancelAttack()
        {
            attackCanceled = true;
            preparingAttack = false;
        }

        private void StartChasing(Transform transform)
        {
            if(!corrupted) return;
            _playerTransform = transform;
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

        protected override IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(attackPreparationTime);
            if(!attackCanceled) Attack();          
            else ResetAttackStoppers();
        }

        protected override void WaitNextAttack()
        {
            if (preparingAttack || !playerReached) return;
            secondsSinceLastAttack += Time.deltaTime;
            if (secondsSinceLastAttack > minTimeBetweenAttacks)
            {
                preparingAttack = true;
                _startRotation = transform.rotation;
                _isRotating = true;
                StartCoroutine(PrepareAttack());
            }
        }

        private void RotateWhilePreparingAttack()
        {
            if (_isRotating)
            {
                var t = _rotationSpeed * Time.deltaTime;
                _rotationAmount += t;
                transform.Rotate(0f, t, 0f);

                if (_rotationAmount > 360f)
                {
                    transform.rotation = _startRotation;
                    _isRotating = false;
                    _rotationAmount = 0f;
                }
            }
        }

        private void LookAtPLayer()
        {
            if (playerReached && !_isRotating)
            {
                transform.LookAt(_playerTransform);
            }
        }

        private void AllowAttack(Transform playerTransform)
        {
            playerReached = true;
        }

        private void ResetAttackStoppers()
        {
            Debug.Log("Tal vez aca");
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
            StartIdle();
        }
        #endregion
    }
}
