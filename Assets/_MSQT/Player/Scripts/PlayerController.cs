using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts.MosquitoBehaviors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MSQT.Player.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerControler : MSQTMono
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float fixedForwardSpeed = 5f;
        [SerializeField] private float moveAcceleration = 30f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float inputRotationScale = 0.5f;
        [SerializeField] private float autoLevelSpeed = 2f;
    
        public IMosquitoDecorator MosquiroBehaviour { get; set; }
        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;

        private Vector2 _lookInput;       // right stick X,Y
        private bool _doBoost;
        private bool _lookInputActive = false;

        private Vector3 _currentVelocity;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            MosquiroBehaviour = new BasicMosquitoBehavior(moveSpeed, rotationSpeed, 100f, 10f);
        }

        private void OnEnable()
        {
            _playerInput.actions["Look"].performed += OnLook;
            _playerInput.actions["Look"].canceled += OnLook;
            _playerInput.actions["Attack"].performed += OnBuzzAttack;
            // playerInput.actions["Jump"].performed += ;
            _playerInput.actions["Boost"].performed += OnBoost;
        }
    
        private void OnDisable()
        {
            _playerInput.actions["Look"].performed -= OnLook;
            _playerInput.actions["Look"].canceled -= OnLook;
            _playerInput.actions["Attack"].performed -= OnBuzzAttack;
            // playerInput.actions["Jump"].performed -= ;
            _playerInput.actions["Boost"].performed -= OnBoost;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 targetLook = context.ReadValue<Vector2>();
                _lookInput = Vector2.Lerp(_lookInput, targetLook, Time.deltaTime * 10f);
                _lookInput.y = Mathf.Clamp(_lookInput.y, -80f, 80f);
                _lookInputActive = true;
            }
            else if (context.canceled)
            {
                _lookInputActive = false;
            }
        }

        public void OnBuzzAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("Buzz attack!");
                // Implement buzz attack logic here
            }
        }

        private void OnBoost(InputAction.CallbackContext context)
        {
            _doBoost = true;
        }

        void FixedUpdate()
        {
            Vector3 desiredVelocity = transform.forward * (fixedForwardSpeed + MosquiroBehaviour.GetMovementSpeed());
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, desiredVelocity, moveAcceleration * Time.fixedDeltaTime);
            _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.fixedDeltaTime);

            // Reduce pitch/roll sensitivity
            float pitch = _lookInput.y * MosquiroBehaviour.GetRotationSpeed() * inputRotationScale * Time.fixedDeltaTime;
            float roll = -_lookInput.x * MosquiroBehaviour.GetRotationSpeed() * inputRotationScale * Time.fixedDeltaTime;

            if (!_lookInputActive)
            {
                // Unified auto-leveling for both pitch and roll using the same smoothing factor and calculation
                float pitchTarget = Mathf.DeltaAngle(transform.eulerAngles.x, 0f);
                float rollTarget = Mathf.DeltaAngle(transform.eulerAngles.z, 0f);

                pitch = Mathf.Lerp(0f, pitchTarget, autoLevelSpeed * Time.fixedDeltaTime);
                roll = Mathf.Lerp(0f, rollTarget, autoLevelSpeed * Time.fixedDeltaTime);
            }

            Quaternion deltaRotation = Quaternion.Euler(pitch, 0f, roll);
            _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);

            if (_doBoost)
            {
                _rigidbody.AddForce(transform.forward * (MosquiroBehaviour.GetMovementSpeed() * 50), ForceMode.Acceleration);
                _doBoost = false;
            }
            else
            {
                // Lerp back to desired velocity for smoother recovery after boost
                _currentVelocity = Vector3.Lerp(_currentVelocity, transform.forward * (fixedForwardSpeed + MosquiroBehaviour.GetMovementSpeed()), Time.fixedDeltaTime * moveAcceleration * 2f);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            // Reflect the current velocity vector on contact normal
            Vector3 bounceDirection = Vector3.Reflect(_currentVelocity.normalized, other.GetContact(0).normal);

            // Apply bounce rotation using the current up direction to maintain local orientation
            Quaternion bounceRotation = Quaternion.LookRotation(bounceDirection, transform.up);
            _rigidbody.MoveRotation(bounceRotation);

            // Optionally zero angular velocity to prevent spinning
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
}