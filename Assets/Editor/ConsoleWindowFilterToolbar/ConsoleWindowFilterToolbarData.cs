using System;
using UnityEngine;

namespace Kogane.Internal
{
    [Serializable]
    internal sealed class ConsoleWindowFilterToolbarData
    {
        [SerializeField] private string m_buttonText;
        [SerializeField] private string m_filteringText;

        public bool   IsValid       => !string.IsNullOrWhiteSpace( m_buttonText );
        public string ButtonText    => m_buttonText;
        public string FilteringText => string.IsNullOrWhiteSpace( m_filteringText ) ? m_buttonText : m_filteringText;
    }
}