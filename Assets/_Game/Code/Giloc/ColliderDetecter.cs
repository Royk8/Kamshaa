using UnityEngine;
using UnityEngine.Events;

namespace Giloc
{
    public class ColliderDetecter : MonoBehaviour
    {
        public UnityAction<Transform> onPlayerDetected;
        public UnityAction onPlayerExit;
        public UnityAction onBossZoneLeaving;

        private void OnTriggerEnter(Collider otherCollider)
        {
            var otherObject = otherCollider.gameObject;
            if (otherObject.CompareTag("Player"))
            {
                onPlayerDetected.Invoke(otherObject.transform);
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (otherCollider.gameObject.CompareTag("Player"))
            {
                onPlayerExit.Invoke();
            }
            else if(otherCollider.gameObject.CompareTag("BossZone")){
                onBossZoneLeaving?.Invoke();
            }
        }
    }
}


