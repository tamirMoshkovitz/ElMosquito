using System;
using _MSQT.Core.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _MSQT.Screens
{
    public class EndingScreen: MSQTMono
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private GameObject camera;

        private InputSystem_Actions _actions;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
        }

        private void Start()
        {
            if (camera) camera.SetActive(true);
        }

        private void OnEnable()
        {
            _actions.UI.Enable();
            EventSystem.current.SetSelectedGameObject(playAgainButton.gameObject);
        }

        private void OnDisable()
        {
            AudioListener.pause = false;
            _actions.UI.Disable();
        }


        public void PlayAgain()
        {
            SceneLoader.LoadScene(SceneName.Opening);
        }

        public void QuitGame()
        {
            AudioListener.pause = false;
            Application.Quit();
        }
    }
}