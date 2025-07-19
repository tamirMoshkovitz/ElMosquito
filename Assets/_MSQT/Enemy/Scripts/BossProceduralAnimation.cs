using System.Collections;
using _MSQT.Core.Scripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace _MSQT.Enemy.Scripts
{
    public class BossProceduralAnimation: MSQTMono
    {
        [Header("Target Settings")]
        [SerializeField] private Transform targetObject;
        [SerializeField] private float reachDistance = 2.0f;

        [Header("Rigging")]
        [SerializeField] private Rig rootRig;
        [SerializeField] private Transform handTarget;
        [SerializeField] private Transform elbowHint;
        [SerializeField] private TwoBoneIKConstraint rightArmIK;

        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private Transform leftElbowHint;
        [SerializeField] private TwoBoneIKConstraint leftArmIK;

        [SerializeField] private Transform head;
        
        [Header("Cinemachine Path")]
        [SerializeField] private CinemachinePathBase rightIkPath;
        [SerializeField] private CinemachinePathBase leftIkPath;


        private Coroutine attackCoroutine;
        private bool isAttacking = false;

        private void Update()
        {
            if (!targetObject || !(rightIkPath is CinemachineSmoothPath smoothPath) || !(leftIkPath is CinemachineSmoothPath leftSmoothPath))
                return;

            float distance = Vector3.Distance(head.position, targetObject.position);
            if (distance <= reachDistance && !isAttacking)
            {
                attackCoroutine = StartCoroutine(PerformAttack(smoothPath, leftSmoothPath));
            }
        }

        private IEnumerator PerformAttack(CinemachineSmoothPath smoothPath, CinemachineSmoothPath leftSmoothPath)
        {
            rootRig.weight = 1;
            isAttacking = true;
            rightIkPath.gameObject.SetActive(true);
            leftIkPath.gameObject.SetActive(true);

            float delay = Random.Range(.1f, 0.3f); // reaction time
            yield return new WaitForSeconds(delay);

            // Update right path waypoint
            if (smoothPath.m_Waypoints.Length > 1)
            {
                Vector3 localTargetPos = smoothPath.transform.InverseTransformPoint(targetObject.position);
                smoothPath.m_Waypoints[1].position = localTargetPos;
                smoothPath.InvalidateDistanceCache();
            }

            // Update left path waypoint
            if (leftSmoothPath.m_Waypoints.Length > 1)
            {
                Vector3 localTargetPosLeft = leftSmoothPath.transform.InverseTransformPoint(targetObject.position);
                leftSmoothPath.m_Waypoints[1].position = localTargetPosLeft;
                leftSmoothPath.InvalidateDistanceCache();
            }

            rightArmIK.weight = 1f;
            leftArmIK.weight = 1f;

            float duration = .5f; // how long it takes to move along the path
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 rightHandPos = smoothPath.EvaluatePositionAtUnit(t, CinemachinePathBase.PositionUnits.Normalized);
                Vector3 leftHandPos = leftSmoothPath.EvaluatePositionAtUnit(t, CinemachinePathBase.PositionUnits.Normalized);
                handTarget.position = rightHandPos;
                leftHandTarget.position = leftHandPos;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Move to final position to ensure end of path
            handTarget.position = smoothPath.EvaluatePositionAtUnit(1f, CinemachinePathBase.PositionUnits.Normalized);
            leftHandTarget.position = leftSmoothPath.EvaluatePositionAtUnit(1f, CinemachinePathBase.PositionUnits.Normalized);

            rightArmIK.weight = 0f;
            leftArmIK.weight = 0f;
            isAttacking = false;
            rightIkPath.gameObject.SetActive(false);
            leftIkPath.gameObject.SetActive(false);
            rootRig.weight = 0;
        }

        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (head)
                Gizmos.DrawWireSphere(head.position, reachDistance);
        }
    }
}