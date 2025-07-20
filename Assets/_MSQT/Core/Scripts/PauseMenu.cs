using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _MSQT.Core.Scripts
{
    public class PauseMenu: MSQTMono
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Button resumeButton;
        
        private static bool _gameIsPaused;
        private InputSystem_Actions _actions;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _actions.UI.Enable();
            _actions.UI.Pause.performed += OnPausePerformed;
        }

        private void OnDisable()
        {
            AudioListener.pause = false;
            _actions.UI.Pause.performed -= OnPausePerformed;
            _actions.UI.Disable();
        }
        
        
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (_gameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _gameIsPaused = false;
            AudioListener.pause = false;
            pauseMenuUI.SetActive(false);
            playerInput.SwitchCurrentActionMap("Player");
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;
            _gameIsPaused = true;
            AudioListener.pause = true;
            pauseMenuUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
            playerInput.SwitchCurrentActionMap("UI");
        }

        public void LoadStartMenu()
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
            _gameIsPaused = false;
            pauseMenuUI.SetActive(false);
            playerInput.SwitchCurrentActionMap("Player");
            SceneManager.LoadScene(0);
            DOTween.KillAll();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}