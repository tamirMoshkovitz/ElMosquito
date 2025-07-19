using UnityEngine.SceneManagement;

namespace _MSQT.Core.Scripts
{
    public enum SceneName
    {
        Opening = 0,
        Tips = 1,
        Game = 2,
        WinEnding = 3,
        LoseEnding = 4,
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