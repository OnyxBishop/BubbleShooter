using System.IO;
using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services.DataSavers
{
    public class PrefsDataService : IDataService
    {
        private readonly JsonSerializer _serializer;

        public PrefsDataService(JsonSerializer serializer) 
            => _serializer = serializer;

        public void Save<TSavable>(TSavable data, bool overwrite = true) 
            where TSavable : ISavable
        {
            if (!overwrite && PlayerPrefs.HasKey(data.SaveName))
                throw new IOException($"File '{data.SaveName}' is already exists and cannot overwritten");
            
            PlayerPrefs.SetString(data.SaveName, _serializer.Serialize(data));
            PlayerPrefs.Save();
        }

        public TSavable Load<TSavable>(string name) 
            where TSavable : ISavable, new()
        {
            if (!IsExists(name))
                return new TSavable();

            var json = PlayerPrefs.GetString(name);

            return _serializer.Deserialize<TSavable>(json);
        }

        public void Delete(string name)
        {
            if (IsExists(name))
                PlayerPrefs.DeleteKey(name);
            else
                Debug.LogWarning($"Data with name '{name}' does not exist, " +
                                 $"but you are trying to delete it");
        }
        
        public bool IsExists(string fileName) 
            => PlayerPrefs.HasKey(fileName);
    }
}