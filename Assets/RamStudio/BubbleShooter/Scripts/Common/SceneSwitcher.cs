using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RamStudio.BubbleShooter.Scripts.Common
{
    public static class SceneSwitcher
    {
        public static AsyncOperation ChangeToAsync(SceneNames sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName.ToString());
        }
    }
}