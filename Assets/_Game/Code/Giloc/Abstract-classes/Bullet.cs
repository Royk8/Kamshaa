using UnityEngine;

namespace Giloc
{
    public abstract class Bullet : MonoBehaviour
    {
        #region properties
        [SerializeField] private float linearSpeed;
        [SerializeField] protected float maxLifeTime;
        [SerializeField] protected float damage;

        #endregion

        #region methods
        protected abstract void Move();
        protected abstract void UpdateLifeState();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var playerHealth =  other.gameObject.GetComponent<IDamageable>();
                playerHealth.ReceiveDamage(damage);
                Destroy(gameObject);
            }
        }
        #endregion

        #region getters / setters
        public float LinearSpeed { get => linearSpeed; set => linearSpeed = value; }
        #endregion
    }
}

