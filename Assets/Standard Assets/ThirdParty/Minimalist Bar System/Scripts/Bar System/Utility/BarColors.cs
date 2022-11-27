using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    [System.Serializable]
    public class BarColors
    {
        // Public properties
        public BarGradient MainFill => _mainFillBarGradient;
        public Color IncrementFill => _incrementFillColor;
        public Color DecrementFill => _decrementFillColor;
        public Color Background => _backgroundColor;
        public Color Border => _borderColor;
        public Color Glow => _glowColor;
        public Color Shadow => _shadowColor;
        public Color FlashColor => _flashColor;
        public Color BackgroundLabelColor => _backgroundLabelColor;
        public Color ForegroundLabelColor => _foregroundLabelColor;

        // Private fields
        [SerializeField] private BarGradient _mainFillBarGradient;
        [SerializeField] private Color _incrementFillColor = Color.white;
        [SerializeField] private Color _decrementFillColor = Color.black;
        [SerializeField] private Color _backgroundColor = new Color(.25f, .25f, .25f);
        [SerializeField] private Color _borderColor = Color.black;
        [SerializeField] private Color _glowColor = new Color(1f, 1f, 1f, .15f);
        [SerializeField] private Color _shadowColor = new Color(0f, 0f, 0f, .5f);
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private Color _backgroundLabelColor = new Color(.85f, .85f, .85f);
        [SerializeField] private Color _foregroundLabelColor = Color.black;
    }
}
