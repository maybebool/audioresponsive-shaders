using LevelManagement.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement.Scenes {
    public class ScenesManager : Singleton<ScenesManager> {
        public void LoadScene(SceneEnum sceneEnum) {
            SceneManager.LoadScene(sceneEnum.ToString());
        }

        public void LoadMainMenu() {
            SceneManager.LoadScene(SceneEnum.MainMenu.ToString());
        }

        public void QuitGame() {
            Application.Quit();
        }
    }
}