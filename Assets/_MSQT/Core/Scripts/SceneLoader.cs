using UnityEngine.SceneManagement;

namespace _MSQT.Core.Scripts
{
    public enum SceneName
    {
        Opening = 0,
        Game = 1,
        WinEnding = 2,
        LoseEnding = 3,
    }

    public static class SceneLoader
    {
        public static void LoadNextScene()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
        }

        public static void LoadScene(SceneName sceneName)
        {
            int index = (int)sceneName;
            if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(index);
            }
        }
    }
}