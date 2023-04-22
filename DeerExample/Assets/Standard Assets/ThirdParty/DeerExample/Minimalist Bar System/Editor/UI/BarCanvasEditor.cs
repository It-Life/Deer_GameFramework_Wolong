using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BarCanvasBhv))]
    public class BarCanvasEditor : Editor
    {
        private SerializedProperty _barRenderMode;
        private SerializedProperty _anchorTransform;
        private SerializedProperty _camera;
        private bool _currentButtonState;
        private bool _previousButtonState;

        private void OnEnable()
        {
            _barRenderMode = serializedObject.FindProperty("_barRenderMode");

            _anchorTransform = serializedObject.FindProperty("_anchorTransform");

            _camera = serializedObject.FindProperty("_camera");
        }

        public override void OnInspectorGUI()
        {
            BarCanvasBhv barCanvas = target as BarCanvasBhv;

            serializedObject.Update();

            EditorExtensions.ScriptHolder(target);

            EditorGUI.BeginChangeCheck();

            EditorExtensions.PropertyField("Render Mode", _barRenderMode);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                barCanvas.UpdateName();

                barCanvas.UpdateCanvas();
            }

            if (barCanvas.RenderMode == RenderMode.World)
            {
                EditorGUI.BeginChangeCheck();

                EditorExtensions.PropertyField("Anchor", _anchorTransform);

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    barCanvas.UpdateCanvas();
                }

                EditorGUI.BeginChangeCheck();

                EditorExtensions.PropertyField("Camera", _camera);

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    barCanvas.UpdateCanvas();
                }

                GUI.enabled = _camera.objectReferenceValue != null;

                GUILayout.BeginHorizontal();

                GUILayout.Space(EditorGUIUtility.labelWidth);

                _currentButtonState = GUILayout.RepeatButton("Preview Runtime Rotation (Look At The Camera)", GUI.skin.button);

                if (Event.current.type == EventType.Repaint && _currentButtonState != _previousButtonState)
                {
                    barCanvas.LookAtCameraIf(_currentButtonState);

                    _previousButtonState = _currentButtonState;
                }

                GUILayout.EndHorizontal();

                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}