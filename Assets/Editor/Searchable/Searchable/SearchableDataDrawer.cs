using System;
using UnityEditor;
using UnityEngine;

namespace RoboRyanTron.SearchableEnum.Editor
{
    [CustomPropertyDrawer(typeof(SearchableData))]
    public  class SearchableDataDrawer : PropertyDrawer
    {
        private int m_IdHash =  "SearchableData".GetHashCode();
        private string[] m_Names = null;
        SerializedProperty m_select = null;
        private bool m_IsInit = false;
        private void Init(SerializedProperty property)
        {
            if (!m_IsInit)
            {
                m_select = property.FindPropertyRelative("m_Select");
                SerializedProperty names = property.FindPropertyRelative("m_Names");
                if (m_Names== null)
                {
                    m_Names = new string[names.arraySize];
                    for (int i = 0; i < m_Names.Length; i++)
                    {
                        m_Names[i] = names.GetArrayElementAtIndex(i).stringValue;
                    }
                }
            }
            m_IsInit = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);
            int id = GUIUtility.GetControlID(m_IdHash, FocusType.Keyboard, position);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
            // If the enum has changed, a blank entry
            if (m_select.intValue < 0 ||m_select.intValue >= m_Names.Length) {
                buttonText = new GUIContent();
            }
            else {
                buttonText = new GUIContent(m_Names[m_select.intValue]);
            }
            
            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    m_select.intValue = i;
                    property.serializedObject.ApplyModifiedProperties();
                };
             
               
                SearchablePopup.Show(position,m_Names , m_select.intValue, onSelect);
            }
            EditorGUI.EndProperty();
        }
        
        /// <summary>
        /// A custom button drawer that allows for a controlID so that we can
        /// sync the button ID and the label ID to allow for keyboard
        /// navigation like the built-in enum drawers.
        /// </summary>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character =='\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }
}