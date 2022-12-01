using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.Quantity
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(QuantityBhv))]
    public class QuantityEditor : Editor
    {
        private SerializedProperty _quantityType;
        private SerializedProperty _maximumAmount;
        private SerializedProperty _minimumAmount;
        private SerializedProperty _currentAmount;
        private SerializedProperty _fillAmount;
        private SerializedProperty _isSegmented;
        private SerializedProperty _segmentAmount;
        private SerializedProperty _segmentCount;
        private SerializedProperty _passiveDynamics;
        private SerializedProperty _onTypeChanged;
        private SerializedProperty _onAmountChanged;
        private SerializedProperty _onInvalidAmount;

        private void OnEnable()
        {
            QuantityBhv quantity = target as QuantityBhv;

            _quantityType = serializedObject.FindProperty("_quantityType");

            _maximumAmount = serializedObject.FindProperty("_maximumAmount");

            _minimumAmount = serializedObject.FindProperty("_minimumAmount");

            _currentAmount = serializedObject.FindProperty("_currentAmount");

            _fillAmount = serializedObject.FindProperty("_fillAmount");

            _isSegmented = serializedObject.FindProperty("_isSegmented");

            _segmentAmount = serializedObject.FindProperty("_segmentAmount");

            _segmentCount = serializedObject.FindProperty("_segmentCount");

            _passiveDynamics = serializedObject.FindProperty("_passiveDynamics");

            _onTypeChanged = serializedObject.FindProperty("_onTypeChanged");

            _onAmountChanged = serializedObject.FindProperty("_onAmountChanged");

            _onInvalidAmount = serializedObject.FindProperty("_onInvalidAmount");

            Undo.undoRedoPerformed += quantity.OnUndoRedoCallback;
        }

        public override void OnInspectorGUI()
        {
            QuantityBhv quantity = target as QuantityBhv;

            serializedObject.Update();

            EditorExtensions.ScriptHolder(target);

            EditorExtensions.Header("Main:");

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Type", _quantityType);

            if (EditorGUI.EndChangeCheck())
            {
                quantity.Type = (QuantityType)_quantityType.intValue;

                quantity.name = BarSystemManager.Instance.automaticObjectNaming ? quantity.Type.ToString() : quantity.name;

                serializedObject.Update();
            }

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Maximum Amount", _maximumAmount);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(quantity, "maximumAmount");

                quantity.MaximumAmount = _maximumAmount.floatValue;

                serializedObject.Update();
            }

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Minimum Amount", _minimumAmount);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(quantity, "minimumAmount");

                quantity.MinimumAmount = _minimumAmount.floatValue;

                serializedObject.Update();
            }

            EditorExtensions.PropertyField("Current Amount", _currentAmount);

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Fill Amount", _fillAmount);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(quantity, "fillAmount");

                quantity.FillAmount = _fillAmount.floatValue;

                serializedObject.Update();
            }

            EditorExtensions.Header("Segmentation:");

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Is Segmented", _isSegmented);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(quantity, "isSegmented");

                quantity.IsSegmented = _isSegmented.boolValue;

                serializedObject.Update();
            }

            if (_isSegmented.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();

                EditorExtensions.PropertyField("Segment Amount", _segmentAmount);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(quantity, "segmentAmount");

                    quantity.SegmentAmount = _segmentAmount.floatValue;

                    serializedObject.Update();
                }

                EditorGUI.BeginChangeCheck();

                EditorExtensions.PropertyField("Segment Count", _segmentCount);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(quantity, "segmentCount");

                    quantity.SegmentCount = _segmentCount.intValue;

                    serializedObject.Update();
                }

                EditorGUI.indentLevel--;
            }

            EditorExtensions.Header("Passive Dynamics:");

            EditorExtensions.PropertyField("Dynamics Type", _passiveDynamics);

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorExtensions.PropertyField("On Type Changed", _onTypeChanged, true);

            EditorExtensions.PropertyField("On Amount Changed", _onAmountChanged, true);

            EditorExtensions.PropertyField("On Invalid Amount", _onInvalidAmount, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}