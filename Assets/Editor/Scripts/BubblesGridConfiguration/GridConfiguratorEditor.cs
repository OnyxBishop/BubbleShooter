#if UNITY_EDITOR

using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.Services.DataSavers;
using RamStudio.BubbleShooter.Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.BubblesGridConfiguration
{
    [CustomEditor(typeof(GridConfigurator))]
    public class GridConfiguratorEditor : UnityEditor.Editor
    {
        private const string ScrollHeight = nameof(ScrollHeight);
        private const string RowHorizontalSpace = nameof(RowHorizontalSpace);
        private const string RowVerticalSpace = nameof(RowVerticalSpace);

        private SerializedProperty _gameConfigurationProperty;
        private SerializedProperty _bubblesArrayProperty;
        private Vector2 _scrollPosition;

        private int _scrollRectHeight;
        private int _rowOffsetX;
        private int _rowOffsetY;
        private int _previousRows = -1;
        private int _previousColumns = -1;
        private string _gridId;
        private bool _isSaveState;

        private void OnEnable()
        {
            _gameConfigurationProperty = serializedObject.FindProperty("_gridDataEditor");
            _bubblesArrayProperty = serializedObject.FindProperty("_bubbles");

            if (_gameConfigurationProperty.objectReferenceValue is GridDataEditor gameConfig)
            {
                _previousRows = gameConfig.MaxRows;
                _previousColumns = gameConfig.MaxColumns;
            }

            _isSaveState = false;
            _scrollRectHeight = EditorPrefs.GetInt(ScrollHeight, 200);
            _rowOffsetX = EditorPrefs.GetInt(RowHorizontalSpace, 30);
            _rowOffsetY = EditorPrefs.GetInt(RowVerticalSpace, 20);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawConfigurationFields();

            var gameConfiguration = (GridDataEditor)_gameConfigurationProperty.objectReferenceValue;
            
            if (gameConfiguration is not null)
                HandleGrid(gameConfiguration);
            else
                ShowWarning();

            if (_isSaveState)
            {
                EditorGUILayout.Space(30);

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField("Grid File Id:", GUILayout.Width(100));
                _gridId = EditorGUILayout.TextField(_gridId);
                
                EditorGUILayout.EndHorizontal();
                
                if (GUILayout.Button("Save", GUILayout.Height(30)))
                {
                    if (string.IsNullOrEmpty(_gridId))
                        EditorUtility.DisplayDialog("Ошибка", "Пожалуйста, введите действительный ID.", "OK");
                    else
                    {
                        SaveBubblesGrid(gameConfiguration.MaxColumns, gameConfiguration.MaxRows);
                        _isSaveState = false;
                    }
                }
            }
            
            if (serializedObject.ApplyModifiedProperties())
            {
                EditorPrefs.SetInt(ScrollHeight, _scrollRectHeight);
                EditorPrefs.SetInt(RowHorizontalSpace, _rowOffsetX);
                EditorPrefs.SetInt(RowVerticalSpace, _rowOffsetY);

                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawConfigurationFields()
        {
            EditorGUILayout.PropertyField(_gameConfigurationProperty, new GUIContent("Game Configuration"));
            _scrollRectHeight = EditorGUILayout.IntField("Scroll Rect Height", _scrollRectHeight);
            _rowOffsetX = EditorGUILayout.IntSlider("Horizontal row space", _rowOffsetX, 30, 60);
            _rowOffsetY = EditorGUILayout.IntSlider("Vertical row space", _rowOffsetY, 10, 30);
        }

        private void HandleGrid(GridDataEditor dataEditor)
        {
            var columns = dataEditor.MaxColumns;
            var rows = dataEditor.MaxRows;
            var total = rows * columns;

            var sizeChanged = rows != _previousRows || columns != _previousColumns;

            if (_bubblesArrayProperty.arraySize != total || sizeChanged)
            {
                if (sizeChanged)
                {
                    InitBubblesArray(total);
                    _previousColumns = columns;
                    _previousRows = rows;
                }
                else
                {
                    InitBubblesArray(total);
                }
            }

            EditorGUILayout.Space();

            DrawGrid(columns, rows);

            EditorGUILayout.Space(40);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset", GUILayout.Height(20)))
                Clear();

            if (GUILayout.Button("Create Grid File", GUILayout.Height(30)))
            {
                _isSaveState = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid(int columns, int rows)
        {
            EditorGUILayout.LabelField("Bubbles Grid", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(_scrollRectHeight));
            
            EditorGUILayout.BeginVertical();
            
            for (var row = 0; row < rows; row++)
            {
                EditorGUILayout.BeginHorizontal();
                
                if(row % 2 == 1)
                    GUILayout.Space(_rowOffsetX);
                
                for (var column = 0; column < columns; column++)
                {
                    var index = row * columns + column;

                    if (index >= _bubblesArrayProperty.arraySize)
                        break;

                    var bubbleProperty = _bubblesArrayProperty.GetArrayElementAtIndex(index);
                    
                    EditorGUILayout.BeginVertical();
                    
                    GUILayout.Space(_rowOffsetY);
                    
                    var bubbleRect = GUILayoutUtility.GetRect(60, EditorGUIUtility.singleLineHeight);

                    EditorGUI.PropertyField(bubbleRect, bubbleProperty, GUIContent.none);
                    GUILayout.Space(10);
                    EditorGUILayout.EndVertical();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void ShowWarning()
        {
            EditorGUILayout.HelpBox("Please assign a Game Configuration to initialize Bubbles.",
                MessageType.Warning);
        }

        private void SaveBubblesGrid(int columns, int rows)
        {
            var array = new BubbleColors[columns * rows];
          
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    var index = row * columns + col;

                    if (index < _bubblesArrayProperty.arraySize)
                        array[index] = (BubbleColors)_bubblesArrayProperty.GetArrayElementAtIndex(index).enumValueIndex;
                    else
                        array[index] = BubbleColors.None;
                }
            }

            var jsonSerializer = new JsonSerializer();
            var fileDataService = new FileDataService(jsonSerializer);
            var saveLoadService = new SaveLoadService(fileDataService);
            
            saveLoadService.SaveGridToFile(array, columns, rows, _gridId);
        }

        private void InitBubblesArray(int newSize)
        {
            var tempArray = new BubbleColors[_bubblesArrayProperty.arraySize];

            for (var i = 0; i < _bubblesArrayProperty.arraySize; i++)
                tempArray[i] = (BubbleColors)_bubblesArrayProperty.GetArrayElementAtIndex(i).enumValueIndex;

            _bubblesArrayProperty.arraySize = newSize;

            for (var i = 0; i < newSize; i++)
            {
                var bubbleProperty = _bubblesArrayProperty.GetArrayElementAtIndex(i);

                if (i < tempArray.Length)
                    bubbleProperty.enumValueIndex = (int)tempArray[i];
                else
                    bubbleProperty.enumValueIndex = (int)BubbleColors.None;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void Clear()
        {
            for (var i = 0; i < _bubblesArrayProperty.arraySize; i++)
            {
                SerializedProperty bubbleProperty = _bubblesArrayProperty.GetArrayElementAtIndex(i);

                if (bubbleProperty != null)
                    bubbleProperty.enumValueIndex = (int)BubbleColors.None;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif