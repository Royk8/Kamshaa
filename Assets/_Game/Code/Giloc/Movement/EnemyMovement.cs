using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giloc.Movement
{
    public class EnemyMovement : MonoBehaviour
    {
        #region properties
        public Action OnPlayerReached;

        [SerializeField] private float chasingSpeed;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float waitTimeBetweenPoints;
        [SerializeField] private List<Transform> idlePoints;

        private Transform _playerTransform;
        private bool _isChasing;
        private float _minDistance;
        private Transform _currentIdleTarget;
        private Vector3 _currentIdleDirection;
        private bool _movementStopped;

        private const float MIN_DISTANCE_TO_IDLE_TARGET = 0.1f;
        #endregion

        #region unity methods
        private void Start()
        {
            _currentIdleTarget = idlePoints[0];
            UpdateIdleDirection();
        }
        void Update()
        {
            MoveChasing();
            MoveIdle();
        }
        #endregion

        #region methods
        public void StartChasing(Transform playerTransform, float minDistance)
        {
            _playerTransform = playerTransform;
            _minDistance = minDistance;
            AllowChasing();
        }

        public void ResumeChasing()
        {
            if (!_playerTransform || _minDistance == 0) return;
            AllowChasing();
        }

        public void StartIdle()
        {
            _playerTransform = null;
            _minDistance = 0;
            _isChasing = false;
            _movementStopped = false;
            UpdateIdleDirection();
        }

        private void MoveIdle()
        {
            if(_movementStopped || _isChasing) return;
            transform.Translate(moveSpeed * Time.deltaTime * _currentIdleDirection);
            if(Vector3.Distance(_currentIdleTarget.position, transform.position) < MIN_DISTANCE_TO_IDLE_TARGET)
            {
                _movementStopped = true;
                StartCoroutine(WaitUntilNextMove());
            }
        }

        private void ChangeIdleTarget()
        {
            var currentIndex = idlePoints.IndexOf(_currentIdleTarget);
            var newIndex = currentIndex == idlePoints.Count - 1 ? 0 : currentIndex + 1;
            _currentIdleTarget = idlePoints[newIndex];
            _currentIdleDirection = (_currentIdleTarget.position - transform.position).normalized;
        }

        private IEnumerator WaitUntilNextMove()
        {
            yield return new WaitForSeconds(waitTimeBetweenPoints);
            ChangeIdleTarget();
            _movementStopped = false;
        }

        private void MoveChasing()
        {
            if (!_isChasing || _movementStopped) return;
            var direction = (_playerTransform.position - transform.position).normalized;
            transform.Translate(chasingSpeed * Time.deltaTime * direction);
            if (Vector3.Distance(transform.position, _playerTransform.position) < _minDistance)
            {
                _movementStopped = true;
                OnPlayerReached.Invoke();
            }
        }

        private void AllowChasing()
        {
            _isChasing = true;
            _movementStopped = false;
        }

        private void UpdateIdleDirection()
        {
            _currentIdleDirection = (_currentIdleTarget.position - transform.position).normalized;
        }
        #endregion

        #region getters/setters
        public bool IsChasing { private get => _isChasing; set => _isChasing = value; }
        #endregion
    }
}

