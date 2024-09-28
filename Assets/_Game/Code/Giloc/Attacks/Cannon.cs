using System.Collections;
using UnityEngine;

namespace Giloc.Attacks
{
    public class Cannon : Attack
    {
        #region properties
        [SerializeField] private GameObject bullet;
        #endregion

        #region methods
        public override void MakeAttack()
        {
            Instantiate(bullet, position: transform.position, rotation: transform.localRotation);
        }
        #endregion
    }
}

