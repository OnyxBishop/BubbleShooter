using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public static class PauseService
    {
        private static bool isPaused = false;

        public static void Pause()
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
            }
        }

        public static void Resume()
        {
            if (isPaused)
            {
                Time.timeScale = 1f;
                isPaused = false;
            }
        }
        
        public static bool IsPaused()
        {
            return isPaused;
        }
    }
}