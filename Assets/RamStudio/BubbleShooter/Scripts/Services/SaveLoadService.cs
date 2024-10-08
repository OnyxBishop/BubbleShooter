using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Boot.Data;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Services.DataSavers;
using RamStudio.BubbleShooter.Scripts.Services.Interfaces;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public class SaveLoadService
    {
        private readonly Dictionary<Type, IDataService> _dataServices;

        public SaveLoadService(params IDataService[] dataServices)
        {
            _dataServices = new Dictionary<Type, IDataService>();
            
            foreach (var dataService in dataServices)
            {
                var type = dataService.GetType();
                _dataServices.TryAdd(type, dataService);
            }
        }

        public void SaveToPrefs(PlayerData playerData) 
            => TryGetService(typeof(PrefsDataService))?.Save(playerData);

        public PlayerData LoadFromPrefs(SaveNames saveName) 
            => TryGetService(typeof(PrefsDataService))?.Load<PlayerData>(saveName.ToString());

        public void SaveGridToFile(BubbleColors[] array, int columns, int rows, string id)
        {
            var gridData = new GridData
            {
                Columns = columns,
                Rows = rows,
                BubblesArray = array,
                Id = id
            };

            TryGetService(typeof(FileDataService))?.Save(gridData);
        }

        public GridData LoadGrid(string id)
        {
            var fileName = $"{SaveNames.Grid}{id}";
            var gridTxt = Resources.Load<TextAsset>($"{AssetPaths.TextAssets}/{fileName}");
            
            return JsonUtility.FromJson<GridData>(gridTxt.text);
        }
        
#if UNITY_EDITOR
        public GridData LoadGrid(string id, bool usingOnlyInEditor)
        {
            var fileName = $"{SaveNames.Grid}{id}";
            return TryGetService(typeof(FileDataService))?.Load<GridData>(fileName);
        }
#endif

        public bool IsExists(Type serviceType, string fullName) 
            => TryGetService(serviceType).IsExists(fullName);

        private IDataService TryGetService(Type serviceType)
        {
            if(!_dataServices.TryGetValue(serviceType, out var dataService))
                Debug.LogError($"Grid save only in file but {nameof(FileDataService)} does not exist");

            return dataService;
        }
    }
}