using System;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    [FilePath( "UserSettings/Deer/ConsoleWindowFilterToolbar.asset", FilePathAttribute.Location.ProjectFolder )]
    internal sealed class ConsoleWindowFilterToolbarSetting : ScriptableSingleton<ConsoleWindowFilterToolbarSetting>
    {
        [SerializeField] private ConsoleWindowFilterToolbarData[] m_list = Array.Empty<ConsoleWindowFilterToolbarData>();

        public ConsoleWindowFilterToolbarData[] List => m_list;

        public void Save()
        {
            Save( true );
        }
    }
}