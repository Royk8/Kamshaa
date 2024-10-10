using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Giloc.Movement
{
    public class EnemyMovement : MonoBehaviour
    {
        #region properties
        public Action<Transform> OnPlayerReached;

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float chasingSpeed;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float waitTimeBetweenPoints;
        [SerializeField] private List<Transform> idlePoints;

        private Transform _playerTransform;
        private bool _isChasing;
        private float _minDistance;
        private Transform _currentIdleTarget;
        private bool _movementStopped;

        private const float MIN_DISTANCE_TO_IDLE_TARGET = 0.1f;
        #endregion

        #region unity methods
        private void Start()
        {
            _currentIdleTarget = idlePoints[0];
        }

        private void Update()
        {
            CheckPlayerDistance();
            CheckPointDistance();
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
            agent.speed = moveSpeed;
            agent.isStopped = false;
            agent.SetDestination(_currentIdleTarget.position);
        }

        private void ChangeIdleTarget()
        {
            var currentIndex = idlePoints.IndexOf(_currentIdleTarget);
            var newIndex = currentIndex == idlePoints.Count - 1 ? 0 : currentIndex + 1;
            _currentIdleTarget = idlePoints[newIndex];
            agent.SetDestination(_currentIdleTarget.position);
        }

        private IEnumerator WaitUntilNextMove()
        {
            yield return new WaitForSeconds(waitTimeBetweenPoints);
            ChangeIdleTarget();
            _movementStopped = false;
        }

        private void CheckPlayerDistance()
        {
            if (!_isChasing || _movementStopped) return;
            if (agent.remainingDistance < _minDistance)
            {
                _movementStopped = true;
                agent.isStopped = true;
                OnPlayerReached.Invoke(_playerTransform);
            }
        }

        private void CheckPointDistance()
        {
            if (_isChasing || _movementStopped) return;
            if(agent.remainingDistance < MIN_DISTANCE_TO_IDLE_TARGET) 
            {
                _movementStopped = true;
                StartCoroutine(WaitUntilNextMove());
            }
        }

        private void AllowChasing()
        {
            _isChasing = true;
            _movementStopped = false;
            agent.isStopped = false;
            agent.speed = chasingSpeed;
            agent.SetDestination(_playerTransform.position);
        }
        #endregion

        #region getters/setters
        public bool IsChasing { private get => _isChasing; set => _isChasing = value; }
        #endregion
    }
}

