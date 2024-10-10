using UnityEngine;
using System.Collections;

namespace Giloc
{
    public abstract class Attack : MonoBehaviour
    {
        #region methods
        public abstract void MakeAttack();
        public abstract void MakeAttack(Transform playerTransform);
        #endregion
    }
}

