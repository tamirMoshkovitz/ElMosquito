using _MSQT.Core.Scripts;
using _MSQT.Player.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _MSQT.Screens
{
    public class TipsScreen: MSQTMono
    {
        [SerializeField] private PlayerInfoManager playerInfoManager;
        private InputSystem_Actions _actions;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
            playerInfoManager.Awake();
        }
        
        private void Start()
        {
            playerInfoManager.Start();

            StartCoroutine(playerInfoManager.UpdateHpBarWithLerp(1));
            StartCoroutine(playerInfoManager.UpdateManeuverBar(1));
            StartCoroutine(playerInfoManager.UpdateDamageBar(1));
        }
        
        private void OnEnable()
        {
            _actions.UI.Enable();
            _actions.UI.Next.performed += OnCloseTipScenePerformed;
        }
        
        private void OnDisable()
        {
            _actions.UI.Next.performed -= OnCloseTipScenePerformed;
            _actions.UI.Disable();
        }
        
        private void OnCloseTipScenePerformed(InputAction.CallbackContext context)
        {
            SceneLoader.LoadNextScene();
        }
    }
}