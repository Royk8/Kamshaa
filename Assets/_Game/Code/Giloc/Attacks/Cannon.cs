using Giloc.Bullets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Giloc.Attacks
{
    public class Cannon : Attack
    {
        #region properties
        [SerializeField] private List<GameObject> bullets;
        [SerializeField] private float timeBetweenShots;
        [SerializeField] private float bulletsToShot;
        [SerializeField] private float minBulletSpeed;
        [SerializeField] private float maxBulletSpeed;
        public Action OnAttackFinished;
        #endregion

        #region methods
        public override void MakeAttack()
        {
            StartCoroutine(ShotRandomBullets());
        }

        public override void MakeAttack(Transform playerTransform)
        {
            StartCoroutine(ShotParabolicBullets(playerTransform));
        }

        private IEnumerator ShotRandomBullets()
        {
            for(int i = 0; i < bulletsToShot; i++)
            {
                var bullet = UnityEngine.Random.Range(0, bullets.Count);
                var bulletCopy = Instantiate(bullets[bullet], position: transform.position, rotation: transform.rotation);
                bulletCopy.GetComponent<Bullet>().LinearSpeed = UnityEngine.Random.Range(minBulletSpeed, maxBulletSpeed);
                yield return new WaitForSeconds(timeBetweenShots);
            }
            OnAttackFinished.Invoke();
        }

        private IEnumerator ShotParabolicBullets(Transform playerTransform)
        {
            for (int i = 0; i < bulletsToShot; i++)
            {
                var bulletClone = Instantiate(bullets[0], position: transform.position, rotation: transform.rotation);
                var acidBullet = bulletClone.GetComponent<AcidBullet>();
                acidBullet.SetTarget(playerTransform);
                yield return new WaitForSeconds(timeBetweenShots);
            }
            OnAttackFinished.Invoke();
        }
        #endregion
    }
}

