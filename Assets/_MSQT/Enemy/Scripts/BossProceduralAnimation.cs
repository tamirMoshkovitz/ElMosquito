using System;
using System.Collections;
using _MSQT.Core.Scripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

        [FormerlySerializedAs("head")] [SerializeField] private Transform reachZoneRoot;
        [SerializeField] private Transform head;
        [SerializeField] private MultiAimConstraint headLookConstraint;
        [SerializeField] private Transform headLookTarget;
        
        [Header("Cinemachine Path")]
        [SerializeField] private CinemachinePathBase rightIkPath;
        [SerializeField] private CinemachinePathBase leftIkPath;


        private Coroutine _attackCoroutine;
        private Quaternion _defaultHeadRotation;

        private void Awake()
        {
            _defaultHeadRotation = head.rotation;
        }

        private void Update()
        {
            if (!targetObject || !(rightIkPath is CinemachineSmoothPath smoothPath) || !(leftIkPath is CinemachineSmoothPath leftSmoothPath))
                return;

            float distance = Vector3.Distance(reachZoneRoot.position, targetObject.position);
            if (distance <= reachDistance && _attackCoroutine == null)
            {
                Vector3 directionToPlayer = targetObject.position - head.position;
                directionToPlayer.y = 0f;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    head.rotation = Quaternion.Slerp(head.rotation, lookRotation, Time.deltaTime * 5f);
                }
                headLookTarget.position = targetObject.position;
                headLookConstraint.weight = 1f;
                _attackCoroutine = StartCoroutine(PerformAttack(smoothPath, leftSmoothPath));
            }
        }

        private IEnumerator PerformAttack(CinemachineSmoothPath smoothPath, CinemachineSmoothPath leftSmoothPath)
        {
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
            
            rootRig.weight = 1;
            rightArmIK.weight = 1f;
            leftArmIK.weight = 1f;

            float duration = .3f; // how long it takes to move along the path
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
            rightIkPath.gameObject.SetActive(false);
            leftIkPath.gameObject.SetActive(false);
            rootRig.weight = 0;
            StartCoroutine(RestoreHeadRotation());
            _attackCoroutine = null;
        }
        
        private IEnumerator RestoreHeadRotation()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Quaternion initialRotation = head.rotation;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                head.rotation = Quaternion.Slerp(initialRotation, _defaultHeadRotation, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            head.rotation = _defaultHeadRotation;
            headLookConstraint.weight = 0f;
        }

        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (reachZoneRoot)
                Gizmos.DrawWireSphere(reachZoneRoot.position, reachDistance);
        }
    }
}