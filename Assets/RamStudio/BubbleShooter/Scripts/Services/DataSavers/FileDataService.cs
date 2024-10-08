using System.IO;
using RamStudio.BubbleShooter.Scripts.Boot.Interfaces;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services.DataSavers
{
    public class FileDataService : IDataService
    {
        private readonly JsonSerializer _serializer;
        private readonly string _dataPath;
        private readonly string _fileFormat;

        public FileDataService(JsonSerializer serializer)
        {
            _serializer = serializer;
            _dataPath = Application.persistentDataPath;
            _fileFormat = "txt";
        }

        public void Save<TSavable>(TSavable data, bool overwrite = true)
            where TSavable : ISavable
        {
            var filePath = GetFilePath(data.SaveName);

            if (!overwrite && IsPathExists(filePath))
                throw new IOException($"File {data.SaveName}.{_fileFormat} " +
                                      $"already exists and cannot be overwritten");

            File.WriteAllText(filePath, _serializer.Serialize(data));
        }
        
        public TSavable Load<TSavable>(string name)
            where TSavable : ISavable, new()
        {
            var dataPath = GetFilePath(name);

            return !IsPathExists(dataPath)
                ? new TSavable()
                : _serializer.Deserialize<TSavable>(File.ReadAllText(dataPath));
        }
        
#if UNITY_EDITOR
        public TSavable Load<TSavable>(string name, bool editorOnly)
            where TSavable : ISavable, new()
        {
            var dataPath = GetFilePath(name);

            return !IsPathExists(dataPath)
                ? new TSavable()
                : _serializer.Deserialize<TSavable>(File.ReadAllText(dataPath));
        }
#endif

        public void Delete(string name)
        {
            var dataPath = GetFilePath(name);

            if (IsExists(dataPath))
                File.Delete(dataPath);
        }

        public bool IsExists(string fileName)
        {
            var path = GetFilePath(fileName);
            return IsPathExists(path);
        }

        private bool IsPathExists(string filePath)
            => File.Exists(filePath);

        private string GetFilePath(string fileName)
            => Path.Combine(_dataPath, string.Concat(fileName, ".", _fileFormat));
    }
}