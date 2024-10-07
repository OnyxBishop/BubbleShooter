using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.BubbleEnumDrawer
{
    [CustomPropertyDrawer(typeof(BubbleColors))]
    public class BubbleColorsDrawer : PropertyDrawer
    {
        private readonly Color _none = Color.grey;
        private readonly Color _red = Color.red;
        private readonly Color _green = Color.green;
        private readonly Color _blue = Color.blue;
        private readonly Color _purple = Color.HSVToRGB(.73f, .72f, .99f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Color enumColor = GetColorForValue((BubbleColors)property.enumValueIndex);
            
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = enumColor;
            
            property.enumValueIndex = EditorGUI.EnumPopup(position, (BubbleColors)property.enumValueIndex).GetHashCode();
            
            GUI.backgroundColor = previousColor;

            EditorGUI.EndProperty();
        }
        
        private Color GetColorForValue(BubbleColors color)
        {
            return color switch
            {
                BubbleColors.None => _none,
                BubbleColors.Red => _red,
                BubbleColors.Green => _green,
                BubbleColors.Blue => _blue,
                BubbleColors.Purple => _purple,
                _ => Color.white
            };
        }
    }
}