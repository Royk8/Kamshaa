using UnityEngine;
namespace Giloc.Bullets
{
    public class SpiralBullet : Bullet
    {
        #region properties
        [SerializeField] [Min(0)]private float rotationSpeed;
        [SerializeField] [Min(0)]private float rotationSlowdownPerSec;
        [SerializeField] private float minRotationSpeed;
        private float localTimer;
        #endregion

        #region unityMethods
        private void Awake()
        {
            Debug.Assert(minRotationSpeed < rotationSpeed, "Minimum rotation speed must be lower than initial rotation speed.");
        }
        private void Update()
        {
            localTimer += Time.deltaTime;
            Move();
            UpdateLifeState();
        }
        #endregion

        #region methods
        protected override void Move()
        {
            var newRotationSpeed = rotationSpeed - (rotationSlowdownPerSec * Time.deltaTime);
            rotationSpeed = Mathf.Clamp(newRotationSpeed, minRotationSpeed, rotationSpeed);
            transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
            transform.Translate(linearSpeed * Time.deltaTime * Vector3.forward, Space.Self);
        }

        protected override void UpdateLifeState()
        {
            if (localTimer > maxLifeTime) {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}

