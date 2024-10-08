using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;

namespace RamStudio.BubbleShooter.Scripts.Services.Interfaces
{
    public interface IDataService
    {
        public void Save<TSavable>(TSavable data, bool overwrite = true)
            where TSavable : ISavable;

        public TSavable Load<TSavable>(string name)
            where TSavable : ISavable, new();

        public bool IsExists(string fileName);
        
        public void Delete(string name);
    }
}