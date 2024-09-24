using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace RamStudio.BubbleShooter.EditorScripts
{
    [CustomEditor(typeof(HexGridGenerator))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            HexGridGenerator generator = (HexGridGenerator)target;

            GUILayout.Space(10);
            
            if(GUILayout.Button("Build Grid"))
            {
                Undo.RecordObject(generator, "Build Grid");
                generator.Build();
                EditorUtility.SetDirty(generator);
            }
            
            if(GUILayout.Button("Clear Grid"))
            {
                Undo.RecordObject(generator, "Clear Grid");
                generator.Clear();
                EditorUtility.SetDirty(generator);
            }
        }
    }
}

#endif