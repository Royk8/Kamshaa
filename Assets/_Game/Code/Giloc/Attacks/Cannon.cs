using Giloc.Bullets;
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
            Instantiate(bullet, position: transform.position, rotation: transform.rotation);
        }

        public override void MakeAttack(Transform playerTransform)
        {
            var bulletClone = Instantiate(bullet, position: transform.position, rotation: transform.rotation);
            var acidBullet = bulletClone.GetComponent<AcidBullet>();
            acidBullet.SetTarget(playerTransform);
        }
        #endregion
    }
}

