using UnityEngine;
using UnityEngine.Events;

namespace Giloc
{
    public class ColliderDetecter : MonoBehaviour
    {
        public UnityAction<Transform> onPlayerDetected;
        public UnityAction onPlayerExit;

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
        }
    }
}


