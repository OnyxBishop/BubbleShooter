using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine.SceneManagement;

namespace RamStudio.BubbleShooter.Scripts.Common
{
    public static class SceneSwitcher
    {
        public static void ChangeToAsync(SceneNames sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName.ToString());
        }
    }
}