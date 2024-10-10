using UnityEngine;

namespace Giloc.Bullets
{
    public class AcidBullet : Bullet
    {
        #region properties
        [SerializeField] private float parabolaHeight;
        [SerializeField] private float timeToHit;
        [SerializeField] private Collider acidArea;
        [SerializeField] private MeshRenderer meshRenderer;
        private Vector3 _targetPoint;
        private Vector3 _controlPoint;
        private Vector3 _startPosition;
        private float _localTimer;
        private float _movementTimer;
        private bool _isMoving;
        #endregion

        #region unity methods
        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            _localTimer += Time.deltaTime;
            Move();
            UpdateLifeState();
        }
        #endregion

        #region methods
        protected override void Move()
        {
            if (!_isMoving) return;
            _movementTimer += Time.deltaTime;
            var t = _localTimer / timeToHit;
            if(_localTimer >= timeToHit)
            {
                _isMoving = false;
                transform.position = _targetPoint;
                HitFloor();
                return;
            }
            var positionOnCurve = Mathf.Pow(1 - t, 2) * _startPosition +
                                  2 * (1 - t) * t * _controlPoint +
                                  Mathf.Pow(t, 2) * _targetPoint;
            transform.position = positionOnCurve;
        }

        protected override void UpdateLifeState()
        {
            if (_localTimer > maxLifeTime)
            {
                Destroy(gameObject);
            }
        }

        private void HitFloor()
        {
            acidArea.enabled = true;
            meshRenderer.enabled = false;
        }

        public void SetTarget(Transform target)
        {
            _targetPoint = target.position;
            var midPoint = Vector3.Lerp(transform.position, _targetPoint, 0.5f);
            _controlPoint = new Vector3(midPoint.x, parabolaHeight + midPoint.y, midPoint.z);
            _movementTimer = 0f;
            _isMoving = true;
        }
        #endregion
    }
}