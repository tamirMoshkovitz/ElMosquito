using _MSQT.Core.Scripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace _MSQT.Enemy.Prefabs.Scripts
{
    public class BossProceduralAnimation: MSQTMono
    {
        [Header("Target Settings")]
        [SerializeField] private Transform targetObject;
        [SerializeField] private float reachDistance = 2.0f;

        [Header("Rigging")]
        [SerializeField] private Transform handTarget;
        [SerializeField] private Transform elbowHint;
        [SerializeField] private TwoBoneIKConstraint rightArmIK;

        [SerializeField] private Transform head;


        void Update()
        {
            if (!targetObject || !rightArmIK || !handTarget || !head) return;

            float distance = Vector3.Distance(head.position, targetObject.position);
            bool isInRange = distance <= reachDistance;

            float targetWeight = isInRange ? 1f : 0f;
            rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, targetWeight, Time.deltaTime * 5f);

            if (isInRange)
            {
                Vector3 p0 = transform.position + transform.right * 0.3f + transform.up * 0.8f; // start at hand
                Vector3 p1 = transform.position + transform.right * 0.8f + transform.up * 1.5f; // arc upward and right
                Vector3 p2 = targetObject.position + targetObject.forward * 0.5f + Vector3.up * 0.2f; // end toward player

                float t = Mathf.PingPong(Time.time * 2.0f, 1.0f);

                // De Casteljau's algorithm for smooth curve
                Vector3 a = Vector3.Lerp(p0, p1, t);
                Vector3 b = Vector3.Lerp(p1, p2, t);
                Vector3 curvePoint = Vector3.Lerp(a, b, t);

                handTarget.position = Vector3.Lerp(handTarget.position, curvePoint, Time.deltaTime * 10f);

                if (elbowHint)
                {
                    Vector3 elbowOffset = transform.right * 0.3f + transform.up * 0.5f;
                    elbowHint.position = Vector3.Lerp(elbowHint.position, transform.position + elbowOffset, Time.deltaTime * 5f);
                }
            }
            else
            {
                // Move the handTarget to a relaxed position near the character's side
                Vector3 restPosition = transform.position + transform.right * 0.3f + transform.up * 0.9f;
                handTarget.position = Vector3.Lerp(handTarget.position, restPosition, Time.deltaTime * 2f);
            }
        }

        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (head)
                Gizmos.DrawWireSphere(head.position, reachDistance);
        }
    }
}