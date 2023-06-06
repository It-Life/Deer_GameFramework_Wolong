using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.Quantity
{
    [CustomPropertyDrawer(typeof(Dynamics))]
    public class DynamicsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty dynamicsType = property.FindPropertyRelative("_dynamicsType");
            SerializedProperty deltaPercentage = property.FindPropertyRelative("_deltaPercentage");
            SerializedProperty deltaTime = property.FindPropertyRelative("_deltaTime");
            SerializedProperty enabled = property.FindPropertyRelative("_enabled");

            GUILayout.Space(-EditorGUIUtility.singleLineHeight);
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            EditorExtensions.PropertyField("", dynamicsType);
            GUILayout.EndHorizontal();

            if (dynamicsType.intValue != (int)DynamicsType.None)
            {
                EditorGUI.indentLevel++;

                EditorExtensions.PropertyField("Delta Percentage", deltaPercentage);
                EditorExtensions.PropertyField("Delta Time", deltaTime);

                GUILayout.BeginHorizontal();
                EditorExtensions.PropertyField("Enabled", enabled, Application.isPlaying, GUILayout.ExpandWidth(false));
                bool previousEnabledState = GUI.enabled;
                GUI.enabled = !Application.isPlaying;
                if (GUILayout.Button("Simulate one time step", GUILayout.ExpandWidth(true)))
                {
                    QuantityBhv quantity = property.serializedObject.targetObject as QuantityBhv;
                    quantity.Amount += quantity.Capacity * quantity.PassiveDynamics.SignedDeltaPercentage;
                }
                GUI.enabled = previousEnabledState;
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}