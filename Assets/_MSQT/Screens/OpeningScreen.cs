using _MSQT.Core.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _MSQT.Screens
{
    public class OpeningScreen: MSQTMono
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private Button playButton;
        
        private InputSystem_Actions _actions;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _actions.UI.Enable();
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        private void OnDisable()
        {
            AudioListener.pause = false;
            _actions.UI.Disable();
        }
        
        
        public void PlayGame()
        {
            SceneLoader.LoadScene(SceneName.Game);
        }

        public void QuitGame()
        {
            AudioListener.pause = false;
            Application.Quit();
        }
    }
}