using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    internal sealed class ConsoleWindowFilterToolbarSettingProvider : SettingsProvider
    {
        public const string PATH = "Deer/ConsoleWindowFilterToolbar";

        private Editor m_editor;

        private ConsoleWindowFilterToolbarSettingProvider
        (
            string              path,
            SettingsScope       scopes,
            IEnumerable<string> keywords = null
        ) : base( path, scopes, keywords )
        {
        }

        public override void OnActivate( string searchContext, VisualElement rootElement )
        {
            var instance = ConsoleWindowFilterToolbarSetting.instance;

            instance.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;

            Editor.CreateCachedEditor( instance, null, ref m_editor );
        }

        public override void OnGUI( string searchContext )
        {
            using var changeCheckScope = new EditorGUI.ChangeCheckScope();

            m_editor.OnInspectorGUI();

            if ( GUILayout.Button( "Refresh" ) )
            {
                ConsoleWindowFilterToolbar.CreateGUI();
                ConsoleWindowFilterToolbar.Setup();
            }

            if ( !changeCheckScope.changed ) return;

            ConsoleWindowFilterToolbarSetting.instance.Save();
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingProvider()
        {
            return new ConsoleWindowFilterToolbarSettingProvider
            (
                path: PATH,
                scopes: SettingsScope.Project
            );
        }
    }
}