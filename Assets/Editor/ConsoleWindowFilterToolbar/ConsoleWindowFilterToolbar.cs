using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class ConsoleWindowFilterToolbar
    {
        private static readonly Type m_type = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.ConsoleWindow" );

        private static VisualElement m_toolbar;
        private static EditorWindow  m_consoleWindow;

        static ConsoleWindowFilterToolbar()
        {
            EditorApplication.delayCall += () => CreateGUI();
            EditorApplication.update += () =>
            {
                if ( m_consoleWindow == null ) return;
                m_toolbar = null;
            };
        }

        public static void CreateGUI()
        {
            if ( m_toolbar != null ) return;

            m_consoleWindow = Resources
                    .FindObjectsOfTypeAll( m_type )
                    .FirstOrDefault() as EditorWindow
                ;

            if ( m_consoleWindow == null ) return;

            m_toolbar = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.RowReverse,
                    top           = 20,
                    right         = 1,
                },
                pickingMode = PickingMode.Ignore,
            };

            var rootVisualElement = m_consoleWindow.rootVisualElement;
            rootVisualElement.Add( m_toolbar );

            Setup();
        }

        public static void Setup()
        {
            if ( m_toolbar == null ) return;

            m_toolbar.Clear();

            var setting = ConsoleWindowFilterToolbarSetting.instance;
            var list    = setting.List;

            if ( list is not { Length: > 0 } ) return;

            foreach ( var data in list.Where( x => x.IsValid ).Reverse() )
            {
                m_toolbar.Add( CreateButton( data.ButtonText, data.FilteringText ) );
            }

            m_toolbar.Add( CreateButton( "x", "" ) );
        }

        private static Button CreateButton( string buttonText, string filteringText )
        {
            return new Button( () => ConsoleWindowInternal.SetFilter( filteringText ) )
            {
                text = buttonText,
                style =
                {
                    marginLeft  = 0,
                    marginRight = 0,
                }
            };
        }
    }
}