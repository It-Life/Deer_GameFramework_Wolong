using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Minimalist.Bar.Utility
{
    [CustomPropertyDrawer(typeof(BarColors))]
    public class BarColorsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUILayout.Space(-EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty mainFillBarGradient = property.FindPropertyRelative("_mainFillBarGradient");
            SerializedProperty incrementFillColor = property.FindPropertyRelative("_incrementFillColor");
            SerializedProperty decrementFillColor = property.FindPropertyRelative("_decrementFillColor");
            SerializedProperty backgroundColor = property.FindPropertyRelative("_backgroundColor");
            SerializedProperty borderColor = property.FindPropertyRelative("_borderColor");
            SerializedProperty glowColor = property.FindPropertyRelative("_glowColor");
            SerializedProperty shadowColor = property.FindPropertyRelative("_shadowColor");
            SerializedProperty flashColor = property.FindPropertyRelative("_flashColor");
            SerializedProperty backgroundLabelColor = property.FindPropertyRelative("_backgroundLabelColor");
            SerializedProperty foregroundLabelColor = property.FindPropertyRelative("_foregroundLabelColor");

            EditorExtensions.PropertyField("Main Fill Gradient", mainFillBarGradient);
            EditorExtensions.PropertyField("Increment Fill Color", incrementFillColor);
            EditorExtensions.PropertyField("Decrement Fill Color", decrementFillColor);
            EditorExtensions.PropertyField("Background Color", backgroundColor);
            EditorExtensions.PropertyField("Border Color", borderColor);
            EditorExtensions.PropertyField("Glow Color", glowColor);
            EditorExtensions.PropertyField("Shadow Color", shadowColor);
            EditorExtensions.PropertyField("Flash Color", flashColor);
            EditorExtensions.PropertyField("Background Label Color", backgroundLabelColor);
            EditorExtensions.PropertyField("Foreground Label Color", foregroundLabelColor);

            EditorGUI.EndProperty();
        }
    }
}