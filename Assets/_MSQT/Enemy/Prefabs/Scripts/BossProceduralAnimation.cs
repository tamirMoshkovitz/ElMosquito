using _MSQT.Core.Scripts;
using Unity.Cinemachine;
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

        [Header("Cinemachine Path")]
        [SerializeField] private CinemachinePathBase ikPath;


        void Update()
        {
            if (!targetObject || !rightArmIK || !handTarget || !head) return;

            float distance = Vector3.Distance(head.position, targetObject.position);
            bool isInRange = distance <= reachDistance;

            float targetWeight = isInRange ? 1f : 0f;
            rightArmIK.weight = Mathf.Lerp(rightArmIK.weight, targetWeight, Time.deltaTime * 5f);

            if (isInRange)
            {
                if (ikPath)
                {
                    // Evaluate a position along the Cinemachine path based on time
                    float pathLength = ikPath.MaxPos;
                    float t = Mathf.PingPong(Time.time * 0.5f, 1.0f); // slower interpolation
                    float pathPosition = Mathf.Lerp(0, pathLength, t);

                    Vector3 pathPoint = ikPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.PathUnits);
                    handTarget.position = Vector3.Lerp(handTarget.position, pathPoint, Time.deltaTime * 10f);
                    handTarget.LookAt(targetObject.position);
                }
                else
                {
                    // Fallback if no path is assigned
                    Vector3 start = handTarget.position;
                    Vector3 mid = targetObject.position + targetObject.forward * 0.5f + Vector3.up * 0.5f;
                    Vector3 end = targetObject.position + targetObject.forward * 0.1f;

                    float t = Mathf.PingPong(Time.time * 1.5f, 1.0f);
                    Vector3 a = Vector3.Lerp(start, mid, t);
                    Vector3 b = Vector3.Lerp(mid, end, t);
                    Vector3 final = Vector3.Lerp(a, b, t);

                    handTarget.position = Vector3.Lerp(handTarget.position, final, Time.deltaTime * 10f);
                    handTarget.LookAt(targetObject.position);
                }

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