using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BarBhv))]
    public class BarEditor : Editor
    {
        private SerializedProperty _quantity;
        private SerializedProperty _barColors;
        private SerializedProperty _mainBorderWidth;
        private SerializedProperty _segmentBorderWidthProportion;

        private bool _barColorsFoldout;
        private bool _barBordersFoldout;

        private void OnEnable()
        {
            _quantity = serializedObject.FindProperty("_quantity");
            _barColors = serializedObject.FindProperty("_barColors");
            _mainBorderWidth = serializedObject.FindProperty("_mainBorderWidth");
            _segmentBorderWidthProportion = serializedObject.FindProperty("_segmentBorderWidthProportion");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BarBhv bar = target as BarBhv;

            this.AnimationTestButtons(bar);

            return;

            serializedObject.Update();

            this.SubscriptionSection(bar);

            this.BarColorsSection(bar);

            this.BarBordersSection(bar);

            this.AnimationTestButtons(bar);
        }

        private void SubscriptionSection(BarBhv bar)
        {
            EditorExtensions.Header("Subscription:");

            EditorExtensions.PropertyField("Quantity", _quantity);
        }

        private void BarColorsSection(BarBhv bar)
        {
            EditorGUILayout.Separator();

            _barColorsFoldout = EditorGUILayout.Foldout(_barColorsFoldout, "Colors:", EditorStyles.foldout);

            if (_barColorsFoldout)
            {
                EditorGUI.BeginChangeCheck();

                EditorExtensions.PropertyField("", _barColors);

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    bar.UpdateColors();
                }
            }
        }

        private void BarBordersSection(BarBhv bar)
        {
            _barBordersFoldout = EditorGUILayout.Foldout(_barBordersFoldout, "Borders:", EditorStyles.foldout);

            if (_barBordersFoldout)
            {
                EditorExtensions.PropertyField("Main Border Width", _mainBorderWidth);

                EditorExtensions.PropertyFieldWithTooltip("Segment Border Width", 
                    "Set as a proportion of the main border's width.",_segmentBorderWidthProportion);
            }
        }

        private void AnimationTestButtons(BarBhv bar)
        {
            GUILayout.Space(8);

            EditorGUILayout.LabelField((Application.isPlaying ? "Runtime" : "Edit Mode") + " Animation Tests:", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUI.enabled = false;

                GUILayout.TextArea("Note that animations may play a bit faster in 'Edit Mode' when compared to their 'Runtime' speeds. " +
                    "For that reason, even though coarse 'Edit Mode' animation tweaking is highly encouraged, " +
                    "I suggest fine-tuning final animation parameters at 'Runtime'.", GUI.skin.textArea);

                GUI.enabled = true;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Decrement By 25%"))
            {
                bar.Quantity.Amount -= bar.Quantity.Capacity * .25f;
            }

            if (GUILayout.Button("Flash"))
            {
                bar.FlashBar();
            }

            if (GUILayout.Button("Increment By 25%"))
            {
                bar.Quantity.Amount += bar.Quantity.Capacity * .25f;
            }

            GUILayout.EndHorizontal();
        }
    }
}