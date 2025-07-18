using System;
using System.Collections;
using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts.MosquitoBehaviors;
using _MSQT.Player.Scripts.MosquitoDecorators;
using _MSQT.Player.Scripts.UI;
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
        
        [Header("Attack")]
        [SerializeField] private float baseDamage = 10f;
        
        [Header("UI")]
        [SerializeField] private PlayerInfoManager infoManager;

        private IMosquitoDecorator _mosquitoBehaviour;
        public IMosquitoDecorator MosquitoBehaviour
        {
            get => _mosquitoBehaviour;
            set
            {
                float oldDamage = _mosquitoBehaviour?.GetDamage() ?? 0f;
                float oldRotationSpeed = _mosquitoBehaviour?.GetRotationSpeed() ?? 0f;
                
                _mosquitoBehaviour = value;
                
                // if Maneuver is high, add speed
                if (_mosquitoBehaviour.GetRotationSpeed() >= MaxManeuver * .9f)
                {
                    _mosquitoBehaviour = new SpeedDecorator(_mosquitoBehaviour);
                }

                float newDamage = _mosquitoBehaviour.GetDamage();
                if (!Mathf.Approximately(oldDamage, newDamage))
                {
                    StartCoroutine(infoManager.UpdateDamageBar(newDamage / MaxDamage));
                }
                
                float newRotationSpeed = _mosquitoBehaviour.GetRotationSpeed();
                if (!Mathf.Approximately(oldRotationSpeed, newRotationSpeed))
                {
                    StartCoroutine(infoManager.UpdateManeuverBar(newRotationSpeed / MaxManeuver));
                }
            }
        }

        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;

        private Vector2 _lookInput;       // right stick X,Y
        private bool _doBoost;
        private bool _lookInputActive;

        private Vector3 _currentVelocity;

        private float _health = 100f;
        private const float MaxHealth = 100f;
        private const float MaxManeuver = 337.5f;
        private const float MaxDamage = 50f;
        public float Health
        {
            get => _health;
            private set
            {
                _health = Math.Clamp(value, 0f, 100f);
                infoManager.SetHP(_health / MaxHealth);
            }
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _mosquitoBehaviour = new BasicMosquitoBehavior(moveSpeed, rotationSpeed, baseDamage);
            infoManager.Awake();
        }

        private void Start()
        {
            infoManager.SetHP(Health / MaxHealth);
            infoManager.SetManeuver(MosquitoBehaviour.GetRotationSpeed() / MaxManeuver);
            infoManager.SetDamage(MosquitoBehaviour.GetDamage() / MaxDamage);
            infoManager.Start();
        }

        private void OnEnable()
        {
            _playerInput.actions["Look"].performed += OnLook;
            _playerInput.actions["Look"].canceled += OnLook;
            _playerInput.actions["Boost"].performed += OnBoost;
        }
    
        private void OnDisable()
        {
            _playerInput.actions["Look"].performed -= OnLook;
            _playerInput.actions["Look"].canceled -= OnLook;
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

        private void OnBoost(InputAction.CallbackContext context)
        {
            _doBoost = true;
        }

        void Update()
        {
            Health = Mathf.Clamp(Health + MosquitoBehaviour.UpdateHP(Time.deltaTime), 0f, MaxHealth);
            infoManager.UpdateHPBar(Health / MaxHealth);
        }

        void FixedUpdate()
        {
            Vector3 desiredVelocity = transform.forward * (fixedForwardSpeed + MosquitoBehaviour.GetMovementSpeed());
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, desiredVelocity, moveAcceleration * Time.fixedDeltaTime);
            _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.fixedDeltaTime);

            // Reduce pitch/roll sensitivity
            float pitch = _lookInput.y * MosquitoBehaviour.GetRotationSpeed() * inputRotationScale * Time.fixedDeltaTime;
            float roll = -_lookInput.x * MosquitoBehaviour.GetRotationSpeed() * inputRotationScale * Time.fixedDeltaTime;

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

            if (_doBoost && _health > 75f)
            {
                _rigidbody.AddForce(transform.forward * (MosquitoBehaviour.GetMovementSpeed() * 50), ForceMode.Acceleration);
                StartCoroutine(GetHurt());
                _doBoost = false;
            }
            else
            {
                // Lerp back to desired velocity for smoother recovery after boost
                _currentVelocity = Vector3.Lerp(_currentVelocity, transform.forward * (fixedForwardSpeed + MosquitoBehaviour.GetMovementSpeed()), Time.fixedDeltaTime * moveAcceleration * 2f);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            MosquitoBehaviour = MosquitoBehaviour.GetPreviousDecorator();
            // Reflect the current velocity vector on contact normal
            Vector3 bounceDirection = Vector3.Reflect(_currentVelocity.normalized, other.GetContact(0).normal);

            // Apply bounce rotation using the current up direction to maintain local orientation
            Quaternion bounceRotation = Quaternion.LookRotation(bounceDirection, transform.up);
            _rigidbody.MoveRotation(bounceRotation);

            // Optionally zero angular velocity to prevent spinning
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;


            StartCoroutine(GetHurt());
        }

        private IEnumerator GetHurt()
        {
            float targetHealth = Health - 10f;
            float elapsedTime = 0f;
            while (elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;
                Health = Mathf.Lerp(Health, targetHealth, elapsedTime / 0.5f);
                infoManager.UpdateHPBar(Health / MaxHealth);
                yield return null;
            }
            Health = targetHealth;
        }
    }
}