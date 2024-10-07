using RamStudio.BubbleShooter.Scripts.Services.DataSavers;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public static class Services
    {
        public static SaveLoadService SaveLoadService { get; private set; }

        public static void BindProjectDependencies()
        {
            BindSaveLoad();
        }

        private static void BindSaveLoad()
        {
            var serializer = new JsonSerializer();
            var prefsDataService = new PrefsDataService(serializer);
            
            SaveLoadService = new SaveLoadService(prefsDataService);
        }
    }
}