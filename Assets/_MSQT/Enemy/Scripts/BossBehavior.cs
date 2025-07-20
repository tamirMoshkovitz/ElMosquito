using System.Collections;
using _MSQT.Core.Scripts;
using UnityEngine;

namespace _MSQT.Enemy.Scripts
{
    public class BossBehavior : MSQTMono
    {
        private static readonly int kStandUpHash = Animator.StringToHash("Stand Up");
        private static readonly int kSpeedHash = Animator.StringToHash("Speed");
        [SerializeField] private RectTransform healthBar;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Transform approachTarget;
        private bool _hasReachedApproachTarget = false;

        private Animator _animator;
        private bool _isChasing = false;
        private float _health = 100f;
        private Vector2 _barSize;
        private bool _isTurning = false;
        private float _lastDirectionX = 0f;

        private void Awake()
        {
            _barSize = healthBar.sizeDelta;
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!healthBar || !healthBar.gameObject || !healthBar.gameObject.activeInHierarchy)
                return;

            float clampedHealth = Mathf.Clamp01(_health / 100f);
            healthBar.sizeDelta = new Vector2(clampedHealth * _barSize.x, healthBar.sizeDelta.y);

            // Approach a predefined target before starting chase
            if (!_isChasing && _health <= 33f && !_hasReachedApproachTarget)
            {
                StandUpBeforeChase();
            }

            else if (_isChasing && playerTransform && !_isTurning)
            {
                ChasePlayer();
            }
        }

        private void StandUpBeforeChase()
        {
            Vector3 targetPos = new Vector3(approachTarget.position.x, transform.position.y, approachTarget.position.z);
            Vector3 moveDir = targetPos - transform.position;

            if (moveDir.magnitude < 0.1f)
            {
                _hasReachedApproachTarget = true;
                StartCoroutine(StartChaseAfterDelay());
            }
            else
            {
                transform.position += moveDir.normalized * (chaseSpeed * Time.deltaTime);
                Quaternion face = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, face, rotationSpeed * Time.deltaTime);
            }
        }

        private void ChasePlayer()
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
            Vector3 direction = new Vector3(targetPosition.x - transform.position.x, 0f, 0f).normalized;

            // Detect direction change and start turn coroutine
            if (direction.x != 0 && !Mathf.Approximately(Mathf.Sign(direction.x), Mathf.Sign(_lastDirectionX)))
            {
                _animator.SetFloat(kSpeedHash, 0f);
                _lastDirectionX = direction.x;
                StartCoroutine(TurnAndPause(direction));
                return;
            }

            // Move boss
            transform.position += direction * (chaseSpeed * Time.deltaTime);
            _animator.SetFloat(kSpeedHash, Mathf.Abs(direction.x));

            // Smoothly rotate to face direction
            if (direction.x != 0)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, direction.x > 0 ? 90f : -90f, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        public void GetHurt(float damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                SceneLoader.LoadScene(SceneName.WinEnding);
            }
        }


        private IEnumerator StartChaseAfterDelay()
        {
            _animator.SetTrigger(kStandUpHash);
            yield return new WaitForSeconds(5f);
            _isChasing = true;
        }



        private IEnumerator TurnAndPause(Vector3 direction)
        {
            _isTurning = true;

            Quaternion targetRotation = Quaternion.Euler(0f, direction.x > 0 ? 90f : -90f, 0f);
            float elapsed = 0f;
            Quaternion startRotation = transform.rotation;

            while (elapsed < 1f)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed);
                elapsed += Time.deltaTime * rotationSpeed;
                yield return null;
            }

            transform.rotation = targetRotation;
            yield return new WaitForSeconds(5f);
            _isTurning = false;
        }
    }
}