using UnityEngine;
namespace Giloc.Bullets
{
    public class SpiralBullet : Bullet
    {
        #region properties
        private float localTimer;
        #endregion

        #region unityMethods
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
            var xPosition = velocity.x * localTimer;
            var zPosition = velocity.z * Mathf.Sqrt(xPosition);

            var localMovement = new Vector3(xPosition, 0, zPosition);
            var worldMovement = transform.TransformDirection(localMovement);

            transform.localPosition += worldMovement;
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

