using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Minimalist.Bar.Utility
{
    public static class EditorExtensions
    {
        public static void ScriptHolder(Object target)
        {
            GUI.enabled = false;

            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), target.GetType(), false);

            GUI.enabled = true;
        }

        public static void Header(string text)
        {
            EditorGUILayout.Separator();

            GUILayout.Label(text, EditorStyles.boldLabel);
        }

        public static void PropertyField(string label, SerializedProperty property, bool enabled = true, params GUILayoutOption[] options)
        {
            GUI.enabled = enabled;

            GUIContent content = new GUIContent(label);

            EditorGUILayout.PropertyField(property, content, options);

            GUI.enabled = true;
        }

        public static void PropertyFieldWithTooltip(string label, string tooltip, SerializedProperty property, bool enabled = true, params GUILayoutOption[] options)
        {
            GUI.enabled = enabled;

            GUIContent content = new GUIContent(label, tooltip);

            EditorGUILayout.PropertyField(property, content, options);

            GUI.enabled = true;
        }

        public static void WriteToEnum<T>(string path, string name, ICollection<T> data, string extension = ".cs")
        {
            using (StreamWriter file = File.CreateText(path + name + extension))
            {
                file.WriteLine("public enum " + name + " \n{");

                int i = 0;

                foreach (var line in data)
                {
                    string lineRep = line.ToString().Replace(" ", string.Empty);

                    if (!string.IsNullOrEmpty(lineRep))
                    {
                        file.WriteLine(string.Format("\t{0} = {1},", lineRep, i));

                        i++;
                    }
                }

                file.WriteLine("\n}");
            }

            AssetDatabase.ImportAsset(path + name + extension);
        }
    }
}