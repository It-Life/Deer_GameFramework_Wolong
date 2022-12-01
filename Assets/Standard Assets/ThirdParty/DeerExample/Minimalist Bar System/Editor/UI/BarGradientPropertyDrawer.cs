using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Minimalist.Bar.Utility
{
    [CustomPropertyDrawer(typeof(BarGradient))]
    public class BarGradientPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUILayout.Space(-EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty gradient = property.FindPropertyRelative("_gradient");

            EditorExtensions.PropertyField(label.text, gradient);

            EditorGUI.EndProperty();
        }
    }
}