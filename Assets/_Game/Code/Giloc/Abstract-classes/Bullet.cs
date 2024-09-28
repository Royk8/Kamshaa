using UnityEngine;

namespace Giloc
{
    public abstract class Bullet : MonoBehaviour
    {
        #region properties
        [SerializeField] protected Vector3 velocity;
        [SerializeField] protected float maxLifeTime;
        #endregion

        #region methods
        protected abstract void Move();
        protected abstract void UpdateLifeState();
        #endregion
    }
}

